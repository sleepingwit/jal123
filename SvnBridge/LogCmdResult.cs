using System.Collections;
using System.Collections.Generic;

namespace Jal123
{
    public class LogCmdResultRevision
    {
        public LogCmdResultRevision(uint revision, string author, string message)
        {
            Revision = revision;
            Author = author;
            Message = message;
        }

        public uint Revision { get; }

        public string Author { get; }

        public string Message { get; }
    }

    public class LogCmdResultItem
    {
        public enum ItemAction : sbyte
        {
            Add = (sbyte) 'A',
            Delete = (sbyte) 'D',
            Replace = (sbyte) 'R',
            Modifiy = (sbyte) 'M'
        }

        public enum ItemNodeKind
        {
            None,
            File,
            Dir,
            Unknown,
            Symlink
        }

        public LogCmdResultItem(string path, ItemAction action, string copyFromPath, uint copyFromRevision,
            ItemNodeKind nodeKind)
        {
            Path = path;
            Action = action;
            CopyFromPath = copyFromPath;
            CopyFromRevision = copyFromRevision;
            NodeKind = nodeKind;
        }

        public string Path { get; }
        public ItemAction Action { get; }
        public string CopyFromPath { get; }
        public uint CopyFromRevision { get; }
        public ItemNodeKind NodeKind { get; }
    }

    public class LogCmdResultItems : IEnumerable<LogCmdResultItem>
    {
        public LogCmdResultItems()
        {
            _items = new List<LogCmdResultItem>();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public void Add(LogCmdResultItem item)
        {
            _items.Add(item);
        }

        public IEnumerator<LogCmdResultItem> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item;
            }
        }

        private readonly List<LogCmdResultItem> _items;
    }

    public class LogCmdResult : IEnumerable<KeyValuePair<LogCmdResultRevision, LogCmdResultItems>>, IEnumerable
    {
        private class ResultComparer : IComparer<LogCmdResultRevision>
        {
            public int Compare(LogCmdResultRevision x, LogCmdResultRevision y)
            {
                if (x.Revision == y.Revision)
                {
                    return 0;
                }

                if (x.Revision > y.Revision)
                {
                    return 1;
                }

                return -1;
            }
        }

        public LogCmdResult()
        {
            _result = new SortedDictionary<LogCmdResultRevision, LogCmdResultItems>(new ResultComparer());
        }

        public int Count
        {
            get { return _result.Count; }
        }

        public void Add(LogCmdResultRevision revision, LogCmdResultItems items)
        {
            _result.Add(revision, items);
        }

        public IEnumerator<KeyValuePair<LogCmdResultRevision, LogCmdResultItems>> GetEnumerator()
        {
            foreach (var kvp in _result)
            {
                yield return kvp;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var kvp in _result)
            {
                yield return kvp;
            }
        }

        private readonly SortedDictionary<LogCmdResultRevision, LogCmdResultItems> _result;
    }
}