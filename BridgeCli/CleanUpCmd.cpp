#include "stdafx.h"
#include "CleanUpCmd.h"

using namespace System::Runtime::InteropServices;
using namespace BridgeCli;

void CleanUpCmd::Run(String^ target)
{
    IntPtr dir = Marshal::StringToHGlobalAnsi(target);
    svn_client_cleanup(reinterpret_cast<const char*>(dir.ToPointer()), ctx, apr_pool);
}

