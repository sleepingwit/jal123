using System;
using System.Collections.Generic;
using System.Text;
using BridgeCli;

namespace Jal123
{
    internal static class LogCmdWrapper
    {
        private class RevisionComparer : IComparer<uint>
        {
            public int Compare(uint x, uint y)
            {
                if (x == y)
                {
                    return 0;
                }

                if (x < y)
                {
                    return 1;
                }

                return -1;
            }
        }

        public static LogCmdResult Log(ICollection<string> urls, ICollection<uint> revisions,
            LogRevisionMode revisionMode = LogRevisionMode.Change,
            Depth depth = Depth.svn_depth_unknown,
            Depth setDepth = Depth.svn_depth_unknown,
            int limit = 0,
            bool stopOnCopy = false,
            bool useMergeHistory = true)
        {
            if (urls.Count <= 0)
            {
                throw new Exception("Empty urls");
            }

            SortedSet<uint> filteredRevisions = new SortedSet<uint>(revisions, new RevisionComparer());
            switch (revisionMode)
            {
                case LogRevisionMode.RevisionRanges:
                    throw new NotImplementedException();
                case LogRevisionMode.Change:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(revisionMode), revisionMode, null);
            }

            LogCmd logCmd = new LogCmd();
            BridgeCliOpt opt = new BridgeCliOpt
            {
                depth = (int) depth,
                set_depth = (int) setDepth,
                limit = limit,
                verbose = Convert.ToInt32(true),
                stop_on_copy = Convert.ToInt32(stopOnCopy),
                use_merge_history = Convert.ToInt32(useMergeHistory)
            };

            List<IntPtr> nativeUtf8 = new List<IntPtr>();
            foreach (string url in urls)
            {
                if (String.IsNullOrEmpty(url))
                {
                    throw new Exception("url is empty");
                }

                nativeUtf8.Add(NativeStringHelper.StringToNativeUtf8(url));
            }

            BridgeCli.LogCmdResult cliResult = logCmd.Run(nativeUtf8, filteredRevisions, opt);

            return ParseCliResult(cliResult);
        }

        private static LogCmdResult ParseCliResult(BridgeCli.LogCmdResult cliResult)
        {
            LogCmdResult logCmdResult = new LogCmdResult();
            foreach (var cliKvp in cliResult.Results)
            {
                LogCmdResultRevision revision = new LogCmdResultRevision(
                    cliKvp.Key.revision,
                    Encoding.UTF8.GetString(cliKvp.Key.author),
                    Encoding.UTF8.GetString(cliKvp.Key.message)
                );
                LogCmdResultItems items = new LogCmdResultItems();
                foreach (var item in cliKvp.Value)
                {
                    string path = Encoding.UTF8.GetString(item.Path);
                    LogCmdResultItem.ItemAction itemAction = LogCmdResultItem.ItemAction.Add;
                    switch (item.Action)
                    {
                        case LogItemAction.Add:
                            itemAction = LogCmdResultItem.ItemAction.Add;
                            break;
                        case LogItemAction.Delete:
                            itemAction = LogCmdResultItem.ItemAction.Delete;
                            break;
                        case LogItemAction.Repalce:
                            itemAction = LogCmdResultItem.ItemAction.Replace;
                            break;
                        case LogItemAction.Modifiy:
                            itemAction = LogCmdResultItem.ItemAction.Modifiy;
                            break;
                        default:
                            throw new Exception(String.Format("Unknown svn path action for {0}:{1}", path,
                                item.Action));
                    }

                    string copyFromPath = Encoding.UTF8.GetString(item.CopyFromPath);
                    uint copyFromRevision = item.CopyFromRev;
                    LogCmdResultItem.ItemNodeKind nodeKind = LogCmdResultItem.ItemNodeKind.Unknown;
                    switch (item.NodeKind)
                    {
                        case SvnNodeKind.None:
                            nodeKind = LogCmdResultItem.ItemNodeKind.None;
                            break;
                        case SvnNodeKind.File:
                            nodeKind = LogCmdResultItem.ItemNodeKind.File;
                            break;
                        case SvnNodeKind.Dir:
                            nodeKind = LogCmdResultItem.ItemNodeKind.Dir;
                            break;
                        case SvnNodeKind.Symlink:
                            nodeKind = LogCmdResultItem.ItemNodeKind.Symlink;
                            break;
                        case SvnNodeKind.Unknown:
                        default:
                            nodeKind = LogCmdResultItem.ItemNodeKind.Unknown;
                            break;
                    }

                    items.Add(new LogCmdResultItem(
                        path, itemAction, copyFromPath, copyFromRevision, nodeKind
                    ));
                }

                logCmdResult.Add(revision, items);
            }

            return logCmdResult;
        }
    }
}