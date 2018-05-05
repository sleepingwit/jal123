#pragma once

#ifndef SVNERROR
#define SVNERROR(expr) \
    do { \
        svn_error_t* err = (expr); \
        if (err) \
        { \
            System::String^ message = gcnew System::String((err)->message); \
            throw gcnew System::Exception(message); \
        } \
    }while(0)
#endif

#include "BridgeCliOpt.h"

namespace BridgeCli
{
    ref class SvnCmd
    {
    public:
        static array<unsigned char>^ GetUtf8Array(const char* buf, size_t len);

        virtual ~SvnCmd();
        !SvnCmd();

    protected:
        SvnCmd();

        void Authenticate();
        void EnsureRevisionRanges(BridgeCliOpt^ opt);

        svn_client_ctx_t* ctx;
        apr_pool_t* apr_pool;
    };
}
