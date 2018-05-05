using System;
using BridgeCli;

namespace Jal123
{
    internal static class ExportCmdWrapper
    {
        public static void Export(string svnFileUrl, uint pegRevision, string targetPath,
            bool force = false,
            bool ignore_externals = false,
            bool ignore_keywords = false)
        {
            if (String.IsNullOrEmpty(svnFileUrl))
            {
                throw new Exception("svnFileUrl is empty");
            }

            if (pegRevision == 0)
            {
                throw new Exception("pegRevision is 0");
            }

            if (String.IsNullOrEmpty(targetPath))
            {
                throw new Exception("targetPath is empty");
            }

            string fileAtPegRevision = String.Format("{0}@{1}", svnFileUrl, pegRevision);

            BridgeCliOpt opt = new BridgeCliOpt
            {
                force = Convert.ToInt32(force),
                ignore_externals = Convert.ToInt32(ignore_externals),
                ignore_keywords = Convert.ToInt32(ignore_keywords)
            };

            ExportCmd cmd = new ExportCmd();
            IntPtr source = NativeStringHelper.StringToNativeUtf8(fileAtPegRevision);
            IntPtr to = NativeStringHelper.StringToNativeUtf8(targetPath);
            cmd.Run(source, to, opt);
        }
    }
}