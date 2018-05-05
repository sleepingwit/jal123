#pragma once

#include "SvnCmd.h"

using namespace System;

namespace BridgeCli
{
    ref class CleanUpCmd sealed : public SvnCmd
    {
    public:
        void Run(String^ target);
    };
}
