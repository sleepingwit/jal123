using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Jal123.GUI
{
    public partial class MainForm : Form
    {
        private class ExportSvnUserState
        {
            public string LabelMessage;
        }

        private class ExportSvnParameters
        {
            public Uri SvnRepository;
            public List<uint> Numbers;
            public string TargetDirectory;
        }

        private enum ExportSvnStep
        {
            Begin = 0,
            CheckLog = 1,
            FilterLog = 2,
            ExportFile = 3,
            BuildDeleteScript = 4,
            WriteExportLog = 5,
            Finish = 6
        }

        public MainForm()
        {
            _svnExportMutex = new object();
            _svnExportLog = new List<string>();
            _svnDeleteLog = new List<string>();
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dlgTargetDirectoryBrowser.Description = "选择SVN导出目录";
            if (cbxSvnRepository.Items.Count > 0)
            {
                cbxSvnRepository.SelectedIndex = 0;
            }

            pbExport.Maximum = Enum.GetValues(typeof(ExportSvnStep)).Length;
            exportSvnBackgroundWorker.DoWork += StartExportSvn;
            exportSvnBackgroundWorker.ProgressChanged += ExportSvnProgressChanged;
            exportSvnBackgroundWorker.RunWorkerCompleted += ExportSvnCompleted;
        }

        private void ExportSvnCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnExport.Enabled = true;

            MessageBox.Show("导出完成", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportSvnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var us = e.UserState as ExportSvnUserState;
            if (us != null)
            {
                pbExport.Value = e.ProgressPercentage + 1;
                lblProgress.Text = us.LabelMessage;
            }
        }

        private void btnTargetDirectory_Click(object sender, EventArgs e)
        {
            if (dlgTargetDirectoryBrowser.ShowDialog() == DialogResult.OK)
            {
                txtTargetDirectory.Text = dlgTargetDirectoryBrowser.SelectedPath;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbxSvnRepository.Text))
            {
                errorProvider.SetError(cbxSvnRepository, "SVN 根路径不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTargetDirectory.Text))
            {
                errorProvider.SetError(txtTargetDirectory, "导出目录不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNumbers.Text) || txtNumbers.Lines.Length == 0)
            {
                errorProvider.SetError(txtNumbers, "请输入要导出的内容");
            }

            errorProvider.Clear();

            var svnRepository = cbxSvnRepository.Text.Trim();
            var svnRepositoryResult = ValidateSvnRepostory(svnRepository);
            if (!string.IsNullOrEmpty(svnRepositoryResult))
            {
                MessageBox.Show(svnRepositoryResult, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var targetDirectory = txtTargetDirectory.Text.Trim();

            var targetResult = ValidateTargetDirectory(targetDirectory);
            if (!string.IsNullOrEmpty(targetResult))
            {
                MessageBox.Show(targetResult, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lock (_svnExportMutex)
            {
                _svnExportLog.Clear();
                _svnDeleteLog.Clear();
            }

            ExportSvnParameters parameters = null;

            // SVN Revision
            if (tbExportWay.SelectedTab == tabPageSvnRevision)
            {
                parameters = ExportSvnDirectly(svnRepository, targetDirectory);
            }

            // 禅道Build
            else if (tbExportWay.SelectedTab == tabPageZentaoBuild)
            {
                parameters = ExportZentaoBuild(svnRepository, targetDirectory);
            }

            // 禅道Bug
            else if (tbExportWay.SelectedTab == tabPageZentaoBug)
            {
                parameters = ExportZentaoBug(svnRepository, targetDirectory);
            }
            else if (tbExportWay.SelectedTab == tabPageZentaoStory)
            {
                parameters = ExportZentaoStory(svnRepository, targetDirectory);
            }

            if (parameters != null)
            {
                btnExport.Enabled = false;
                exportSvnBackgroundWorker.RunWorkerAsync(parameters);
            }
        }

        private ExportSvnParameters ExportZentaoStory(string svnRepository, string targetDirectory)
        {
            var stories = new List<int>();
            foreach (var rawStory in txtZentaoStory.Lines)
            {
                if (string.IsNullOrWhiteSpace(rawStory))
                {
                    continue;
                }

                var rawStories = rawStory.Split(',');
                foreach (var raw in rawStories)
                {
                    if (!int.TryParse(raw.Trim(), out var story))
                    {
                        MessageBox.Show(string.Format("{0} 不是有效的数字", rawStory.Trim()), "错误", MessageBoxButtons.OK);
                        return null;
                    }

                    stories.Add(story);
                }
            }

            if (stories.Count <= 0)
            {
                MessageBox.Show("没有输入禅道需求号", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var storyExporter = new ZentaoDbStoryRevisionExporter();
            var storyNumbers = storyExporter.Export(stories);

            var filter = new HashSet<uint>();
            foreach (var rawBugNumber in storyNumbers)
            {
                if (!filter.Contains(rawBugNumber))
                {
                    filter.Add(rawBugNumber);
                }
            }

            var svnRoot = new Uri(svnRepository);
            var parameters = new ExportSvnParameters
            {
                Numbers = new List<uint>(filter),
                SvnRepository = svnRoot,
                TargetDirectory = targetDirectory
            };
            return parameters;
        }

        private ExportSvnParameters ExportZentaoBug(string svnRepository, string targetDirectory)
        {
            var bugs = new List<int>();
            foreach (var rawBug in txtZentaoBug.Lines)
            {
                if (string.IsNullOrWhiteSpace(rawBug))
                {
                    continue;
                }

                var rawBugs = rawBug.Split(',');
                foreach (var raw in rawBugs)
                {
                    if (!int.TryParse(raw.Trim(), out var bug))
                    {
                        MessageBox.Show(string.Format("{0} 不是有效的数字", rawBug.Trim()), "错误", MessageBoxButtons.OK);
                        return null;
                    }

                    bugs.Add(bug);
                }
            }

            if (bugs.Count <= 0)
            {
                MessageBox.Show("没有输入禅道Bug号", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var bugExporter = new ZentaoDbBugRevisionExporter();
            var bugNumbers = bugExporter.Export(bugs);

            var filter = new HashSet<uint>();
            foreach (var rawBugNumber in bugNumbers)
            {
                if (!filter.Contains(rawBugNumber))
                {
                    filter.Add(rawBugNumber);
                }
            }

            var svnRoot = new Uri(svnRepository);
            var parameters = new ExportSvnParameters
            {
                Numbers = new List<uint>(filter),
                SvnRepository = svnRoot,
                TargetDirectory = targetDirectory
            };
            return parameters;
        }

        private ExportSvnParameters ExportZentaoBuild(string svnRepository, string targetDirectory)
        {
            var data = cbxZentaoBuild.SelectedItem as ZentaoBuildData;
            if (data == null)
            {
                return null;
            }

            var rawNumbers = new List<uint>();

            var storyExporter = new ZentaoDbStoryRevisionExporter();
            var storyNumbers = storyExporter.Export(data.Stories);
            rawNumbers.AddRange(storyNumbers);

            var bugExporter = new ZentaoDbBugRevisionExporter();
            var bugNumbers = bugExporter.Export(data.Bugs);
            rawNumbers.AddRange(bugNumbers);

            var filter = new HashSet<uint>();
            foreach (var rawNumber in rawNumbers)
            {
                if (!filter.Contains(rawNumber))
                {
                    filter.Add(rawNumber);
                }
            }

            var svnRoot = new Uri(svnRepository);
            var parameters = new ExportSvnParameters
            {
                Numbers = new List<uint>(filter),
                SvnRepository = svnRoot,
                TargetDirectory = targetDirectory
            };
            return parameters;
        }

        private ExportSvnParameters ExportSvnDirectly(string svnRepository, string targetDirectory)
        {
            var numbers = new List<uint>();
            foreach (var rawNumber in txtNumbers.Lines)
            {
                if (string.IsNullOrWhiteSpace(rawNumber))
                {
                    continue;
                }

                var rawNumbers = rawNumber.Split(',');
                foreach (var raw in rawNumbers)
                {
                    if (!uint.TryParse(raw.Trim(), out var number))
                    {
                        MessageBox.Show(string.Format("{0} 不是有效的数字", rawNumber.Trim()), "错误", MessageBoxButtons.OK);
                        return null;
                    }

                    numbers.Add(number);
                }
            }

            if (numbers.Count <= 0)
            {
                MessageBox.Show("没有输入版本号", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var svnRoot = new Uri(svnRepository);

            var parameters = new ExportSvnParameters
            {
                Numbers = numbers,
                SvnRepository = svnRoot,
                TargetDirectory = targetDirectory
            };
            return parameters;
        }

        private void StartExportSvn(object sender, DoWorkEventArgs e)
        {
            var userState = new ExportSvnUserState();
            userState.LabelMessage = "检索SVN日志";

            var parameters = e.Argument as ExportSvnParameters;

            var bg = sender as BackgroundWorker;
            bg.ReportProgress((int) ExportSvnStep.Begin, userState);

            try
            {
                var logs = SvnRevisionsExportor.GetSvnLog(parameters.SvnRepository, parameters.Numbers);
                bg.ReportProgress((int) ExportSvnStep.CheckLog, userState);

                userState.LabelMessage = "重建日志文件索引";
                var files = SvnRevisionsExportor.RebuildFileUrls(logs);
                bg.ReportProgress((int) ExportSvnStep.FilterLog, userState);

                userState.LabelMessage = "执行导出";
                SvnRevisionsExportor.SvnFileExported += OnSvnFileExported;
                SvnRevisionsExportor.Export(parameters.TargetDirectory, parameters.SvnRepository, files);
                SvnRevisionsExportor.SvnFileExported -= OnSvnFileExported;
                bg.ReportProgress((int) ExportSvnStep.ExportFile, userState);

                userState.LabelMessage = "生成删除脚本";
                MakeDeleteScript(parameters.TargetDirectory);
                bg.ReportProgress((int) ExportSvnStep.BuildDeleteScript, userState);

                userState.LabelMessage = "生成导出日志";
                WriteExportLog(parameters.TargetDirectory);
                bg.ReportProgress((int) ExportSvnStep.WriteExportLog, userState);
            }
            catch (Exception exception)
            {
                var sb = new StringBuilder();
                sb.AppendLine(exception.Message);
                sb.Append(exception.StackTrace);
                MessageBox.Show(sb.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            userState.LabelMessage = "导出完成";
            bg.ReportProgress((int) ExportSvnStep.Finish, userState);
        }

        private void OnSvnFileExported(object sender, SvnFileExportedEventArgs e)
        {
            lock (_svnExportMutex)
            {
                _svnExportLog.Add(string.Format("{0}\t{1}@{2}\r\n", e.Action, e.SvnFile, e.Revision));
                if (e.Action == LogCmdResultItem.ItemAction.Delete)
                {
                    _svnDeleteLog.Add(e.SvnFile);
                }
            }
        }

        private void MakeDeleteScript(string targetDirectory)
        {
            lock (_svnExportMutex)
            {
                var path = Path.Combine(targetDirectory, @"Delete.bat");
                var fs = new FileStream(path, FileMode.Create);
                var sw = new StreamWriter(fs, Encoding.ASCII);
                sw.WriteLine("@echo off");
                foreach (var log in _svnDeleteLog)
                {
                    sw.WriteLine("echo del /F /Q \"{0}\"",
                        log.TrimStart('/').Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
                    sw.WriteLine("del /F /Q \"{0}\"",
                        log.TrimStart('/').Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
                }

                sw.WriteLine("@echo on");
                sw.WriteLine("pause");

                sw.Close();
            }
        }

        private void WriteExportLog(string targetDirectory)
        {
            lock (_svnExportMutex)
            {
                _svnExportLog.Sort();
                var path = Path.Combine(targetDirectory, @"ExportLog.txt");
                var fs = new FileStream(path, FileMode.Create);
                var sw = new StreamWriter(fs, Encoding.UTF8);
                foreach (var log in _svnExportLog)
                {
                    sw.Write(log);
                }

                sw.Close();
            }
        }

        private string ValidateSvnRepostory(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "SVN根目录不能为空";
            }

            try
            {
                var match = Regex.Match(url, @"^SVN\://", RegexOptions.IgnoreCase);
                if (match.Length <= 0)
                {
                    return "SVN根目录应当以svn://开头";
                }

                var uri = new Uri(url);
                if (!uri.IsWellFormedOriginalString())
                {
                    return "SVN根目录不是有效的URI格式";
                }
            }
            catch (Exception e)
            {
                return "SVN根目录含有非法字符";
            }

            return null;
        }

        // 这是一个很简单的本地目录路径验正方法
        private string ValidateTargetDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "导出目录的路径不能为空";
            }

            try
            {
                var match = Regex.Match(path, @"^[A-Za-z]\:\\");
                if (match.Length <= 0)
                {
                    return "导出目录的路径不能是相对路径";
                }

                var fullPath = Path.GetFullPath(path);
                if (!Directory.Exists(fullPath))
                {
                    return "导出目录不存在";
                }

                if (Directory.GetDirectories(fullPath).Length > 0
                    || Directory.GetFiles(fullPath).Length > 0)
                {
                    return "导出目录含有内容，请选择一个空目录导出";
                }
            }
            catch (Exception e)
            {
                return "导出目录的路径含有非法字符";
            }

            return null;
        }

        private void btnRequestZentaoProduct_Click(object sender, EventArgs e)
        {
            var exporter = new ZentaoDbProductsExporter();
            var results = exporter.Export();
            cbxZentaoProduct.Items.Clear();
            foreach (var zentaoProductData in results)
            {
                cbxZentaoProduct.Items.Add(zentaoProductData);
            }

            if (cbxZentaoProduct.Items.Count > 0)
            {
                cbxZentaoProduct.SelectedIndex = 0;
            }
        }

        private void cbxZentaoProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxZentaoProject.Items.Clear();

            if (cbxZentaoProduct.SelectedItem != null)
            {
                var exporter = new ZentaoDbProjectsExporter();
                var results = exporter.Export(cbxZentaoProduct.SelectedItem as ZentaoProductData);
                foreach (var zentaoProjectData in results)
                {
                    cbxZentaoProject.Items.Add(zentaoProjectData);
                }

                if (cbxZentaoProject.Items.Count > 0)
                {
                    cbxZentaoProject.SelectedIndex = 0;
                }
            }
        }

        private void cbxZentaoProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxZentaoBuild.Items.Clear();
            if (cbxZentaoProject.SelectedItem != null)
            {
                var exporter = new ZentaoDbBuildsExporter();
                var results = exporter.Export(cbxZentaoProject.SelectedItem as ZentaoProjectData);

                foreach (var zentaoBuildData in results)
                {
                    cbxZentaoBuild.Items.Add(zentaoBuildData);
                }

                if (cbxZentaoBuild.Items.Count > 0)
                {
                    cbxZentaoBuild.SelectedIndex = 0;
                }
            }
        }

        private readonly List<string> _svnExportLog;
        private readonly List<string> _svnDeleteLog;

        private readonly object _svnExportMutex;
    }
}