#pragma once

#include "SvnCmd.h"

using namespace System;

namespace BridgeCli
{
    ref class ExportCmd sealed : public SvnCmd
    {
    public:
        void Run(IntPtr fileUrlAtPegRevision, IntPtr destPath, BridgeCliOpt^ opt);
        void Run(IntPtr fileUrlAtPegRevision, IntPtr destPath, BridgeCliOpt^ opt, unsigned int revision);
    };
}
