using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Jal123
{
    public class SvnFileExportedEventArgs : EventArgs
    {
        public string SvnFile { get; set; }
        public uint Revision { get; set; }
    }

    public static class SvnRevisionsExportor
    {
        public static event EventHandler<SvnFileExportedEventArgs> SvnFileExported;

        private const string SvnExecutableName = "svn.exe";
        private const string SvnExportCmd = "export \"{0}@{1}\" \"{2}\"";

        public static LogCmdResult GetSvnLog(Uri svnRepository, ICollection<uint> revisions)
        {
            List<string> urls = new List<string>();
            urls.Add(svnRepository.ToString());

            return LogCmdWrapper.Log(urls, revisions);
        }

        public static Dictionary<string, uint> RebuildFileUrls(LogCmdResult logCmdResult)
        {
            Dictionary<string, uint> files = new Dictionary<string, uint>();
            foreach (var kvp in logCmdResult)
            {
                foreach (var resultItem in kvp.Value)
                {
                    if (!files.ContainsKey(resultItem.Path))
                    {
                        if (resultItem.Action != LogCmdResultItem.ItemAction.Delete
                            && resultItem.NodeKind == LogCmdResultItem.ItemNodeKind.File)
                            files.Add(resultItem.Path, kvp.Key.Revision);
                    }
                    else
                    {
                        if (kvp.Key.Revision > files[resultItem.Path])
                        {
                            if (resultItem.Action == LogCmdResultItem.ItemAction.Delete)
                            {
                                files.Remove(resultItem.Path);
                            }
                            else
                            {
                                files[resultItem.Path] = kvp.Key.Revision;
                            }
                        }
                    }
                }
            }

            return files;
        }

        public static void Export(string targetDiretory, Uri svnRoot, Dictionary<string, uint> files)
        {
            foreach (var item in files)
            {
                // 跳过服务器代码
                if (item.Key.Contains("Owl"))
                {
                    continue;
                }
                string altPath = item.Key.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                altPath = altPath.TrimStart('\\');
                string localFile = Path.Combine(targetDiretory, altPath);

                Uri svnFile = new Uri(svnRoot, svnRoot.LocalPath.TrimEnd('/') + item.Key);

                Directory.CreateDirectory(Path.GetDirectoryName(localFile));
                ExportCmdWrapper.Export(Uri.EscapeUriString(svnFile.ToString()), item.Value, localFile);
                //string cmd = Path.Combine(Environment.CurrentDirectory, SvnExecutableName);
                //string exportCmd = String.Format(SvnExportCmd, svnFile.ToString(), item.Value, localFile);

                //Process process = new Process();
                //process.StartInfo.FileName = cmd;
                //process.StartInfo.Arguments = exportCmd;
                //process.StartInfo.CreateNoWindow = true;
                //process.StartInfo.UseShellExecute = false;
                //process.Start();
                //process.WaitForExit();
                //if (process.ExitCode != 0)
                //{
                //    throw new Exception(String.Format("File {0} export failed.", svnFile));
                //}
                SvnFileExportedEventArgs e = new SvnFileExportedEventArgs
                {
                    SvnFile = svnFile.ToString(),
                    Revision = item.Value
                };
                OnSvnFileExported(e);
            }
        }

        private static void OnSvnFileExported(SvnFileExportedEventArgs args)
        {
            EventHandler<SvnFileExportedEventArgs> handler = SvnFileExported;
            if (handler != null)
            {
                handler(null, args);
            }
        }
    }
}