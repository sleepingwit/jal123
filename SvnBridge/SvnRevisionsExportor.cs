using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Jal123
{
    public class SvnFileExportedEventArgs : EventArgs
    {
        public string SvnFile { get; set; }
        public uint Revision { get; set; }
        public LogCmdResultItem.ItemAction Action { get; set; }
    }

    public class SvnFileInfo
    {
        public uint Revision { get; set; }
        public LogCmdResultItem.ItemNodeKind NodeKind { get; set; }
        public LogCmdResultItem.ItemAction Action { get; set; }
    }

    public static class SvnRevisionsExportor
    {
        private const string SvnExecutableName = "svn.exe";
        private const string SvnExportCmd = "export \"{0}@{1}\" \"{2}\"";
        public static event EventHandler<SvnFileExportedEventArgs> SvnFileExported;

        public static LogCmdResult GetSvnLog(Uri svnRepository, ICollection<uint> revisions)
        {
            var urls = new List<string>();
            urls.Add(svnRepository.ToString());

            return LogCmdWrapper.Log(urls, revisions);
        }

        public static Dictionary<string, SvnFileInfo> RebuildFileUrls(LogCmdResult logCmdResult)
        {
            var files = new Dictionary<string, SvnFileInfo>();

            foreach (var kvp in logCmdResult)
            {
                foreach (var resultItem in kvp.Value)
                {
                    if (!files.ContainsKey(resultItem.Path))
                    {
                        var info = new SvnFileInfo
                        {
                            Revision = kvp.Key.Revision,
                            NodeKind = resultItem.NodeKind,
                            Action = resultItem.Action
                        };
                        files.Add(resultItem.Path, info);
                    }
                    else
                    {
                        var info = files[resultItem.Path];
                        if (kvp.Key.Revision > info.Revision)
                        {
                            var newInfo = new SvnFileInfo
                            {
                                Revision = kvp.Key.Revision,
                                NodeKind = resultItem.NodeKind,
                                Action = resultItem.Action
                            };
                            files[resultItem.Path] = newInfo;
                        }
                    }
                }
            }

            return files;
        }

        public static void Export(string targetDiretory, Uri svnRoot, Dictionary<string, SvnFileInfo> files)
        {
            foreach (var item in files)
            {
                if (item.Value.Action != LogCmdResultItem.ItemAction.Delete &&
                    item.Value.NodeKind == LogCmdResultItem.ItemNodeKind.File)
                {
                    var altPath = item.Key.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                    altPath = altPath.TrimStart('\\');
                    var localFile = Path.Combine(targetDiretory, altPath);

                    var svnFile = new Uri(svnRoot, svnRoot.LocalPath.TrimEnd('/') + item.Key);

                    Directory.CreateDirectory(Path.GetDirectoryName(localFile));
                    //ExportCmdWrapper.Export(Uri.EscapeUriString(svnFile.ToString()), item.Value.Revision, localFile);

                    string cmd = Path.Combine(Environment.CurrentDirectory, SvnExecutableName);
                    string exportCmd = String.Format(SvnExportCmd, svnFile.ToString(), item.Value.Revision, localFile);

                    Process process = new Process();
                    process.StartInfo.FileName = cmd;
                    process.StartInfo.Arguments = exportCmd;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        throw new Exception(String.Format("File {0} export failed.", svnFile));
                    }
                }

                var e = new SvnFileExportedEventArgs
                {
                    SvnFile = item.Key,
                    Revision = item.Value.Revision,
                    Action = item.Value.Action
                };
                OnSvnFileExported(e);
            }
        }

        private static void OnSvnFileExported(SvnFileExportedEventArgs args)
        {
            var handler = SvnFileExported;
            if (handler != null)
            {
                handler(null, args);
            }
        }
    }
}