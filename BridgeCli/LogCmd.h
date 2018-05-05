#pragma once

#include <map>
#include <vector>
#include <memory>
#include "SvnCmd.h"

using namespace System;
using namespace System::Collections::Generic;

namespace BridgeCli
{
    private enum class SvnNodeKind : int
    {
        None,
        File,
        Dir,
        Unknown,
        Symlink
    };

    private enum class LogItemAction : char
    {
        Add = 'A',
        Delete = 'D',
        Repalce = 'R',
        Modifiy = 'M',
    };

    ref struct LogCmdResultRevisionItem : System::IEquatable<LogCmdResultRevisionItem^>
    {
        unsigned int revision;
        array<unsigned char>^ author;
        array<unsigned char>^ message;

    public:
        virtual bool Equals(LogCmdResultRevisionItem^ other) 
        {
            return this->revision == other->revision;
        }
    };

    ref struct LogCmdResultPathItem
    {
        array<unsigned char>^ Path;
        LogItemAction Action;
        array<unsigned char>^ CopyFromPath;
        unsigned int CopyFromRev;
        SvnNodeKind NodeKind;
    };

    ref class LogCmdResult sealed
    {
    public:
        property Dictionary<LogCmdResultRevisionItem^, List<LogCmdResultPathItem^>^>^ Results
        {
            Dictionary<LogCmdResultRevisionItem^, List<LogCmdResultPathItem^>^>^ get()
            {
                return _results;
            }
        }

    private:
        Dictionary<LogCmdResultRevisionItem^, List<LogCmdResultPathItem^>^>^ _results 
            = gcnew Dictionary<LogCmdResultRevisionItem^, List<LogCmdResultPathItem^>^>();
    };

    ref class LogCmd sealed : public SvnCmd
    {
    public:
        LogCmdResult ^ Run(ICollection<IntPtr>^ urls, ICollection<unsigned int>^ revisions, BridgeCliOpt^ opt);

    private:
        void ParseTargets(apr_array_header_t** targets_p, ICollection<IntPtr>^ urls);
    };
}