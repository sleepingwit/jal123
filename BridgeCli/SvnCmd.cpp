#include "stdafx.h"
#include "SvnCmd.h"

using namespace BridgeCli;

SvnCmd::SvnCmd() :ctx(nullptr)
, apr_pool(nullptr)
{
    apr_initialize();
    svn_dso_initialize2();
    apr_pool = apr_allocator_owner_get(svn_pool_create_allocator(FALSE));
    if (!apr_pool)
    {
        throw gcnew System::Exception();
    }
    svn_utf_initialize2(FALSE, apr_pool);
    svn_nls_init();

    SVNERROR(svn_ra_initialize(apr_pool));

    SVNERROR(svn_config_ensure(nullptr, apr_pool));

    apr_hash_t* cfg_hash = nullptr;
    SVNERROR(svn_config_get_config(&cfg_hash, nullptr, apr_pool));

    svn_client_ctx_t* ctxTemp = nullptr;
    SVNERROR(svn_client_create_context2(&ctxTemp, cfg_hash, apr_pool));
    ctx = ctxTemp;
    if (!ctx)
    {
        throw gcnew System::Exception();
    }
}

SvnCmd::~SvnCmd()
{
    this->!SvnCmd();
}

SvnCmd::!SvnCmd()
{
    svn_pool_destroy(apr_pool);
    apr_pool = nullptr;
}

array<unsigned char>^ SvnCmd::GetUtf8Array(const char* buf, size_t len)
{
    cli::array<unsigned char>^ arr = gcnew cli::array<unsigned char>(len);
    for (size_t i = 0; i < len; i++)
    {
        arr[i] = *(buf + i);
    }

    return arr;
}

void SvnCmd::EnsureRevisionRanges(BridgeCliOpt^ opt)
{
    if (opt->revision_ranges)
        return;

    opt->revision_ranges = apr_array_make(apr_pool, 0, sizeof(svn_opt_revision_range_t*));

    svn_opt_revision_range_t* range = (svn_opt_revision_range_t*)apr_palloc(apr_pool, sizeof(svn_opt_revision_range_t));
    range->start.kind = svn_opt_revision_unspecified;
    range->end.kind = svn_opt_revision_unspecified;
    APR_ARRAY_PUSH(opt->revision_ranges, svn_opt_revision_range_t*) = range;

    opt->start_revision = &(APR_ARRAY_IDX(opt->revision_ranges, 0, svn_opt_revision_range_t*)->start);
    opt->end_revision = &(APR_ARRAY_IDX(opt->revision_ranges, 0, svn_opt_revision_range_t*)->end);
}

void SvnCmd::Authenticate()
{
    svn_auth_baton_t* ab = nullptr;
    apr_array_header_t* providers = nullptr;
    svn_auth_provider_object_t* provider = nullptr;
    svn_config_t* cfg_config = (svn_config_t*)svn_hash_gets(ctx->config, SVN_CONFIG_CATEGORY_CONFIG);
    
    SVNERROR(svn_auth_get_platform_specific_client_providers(&providers, cfg_config, apr_pool));

    svn_auth_get_simple_provider2(&provider, nullptr, nullptr, apr_pool);
    APR_ARRAY_PUSH(providers, svn_auth_provider_object_t*) = provider;

    svn_auth_get_username_provider(&provider, apr_pool);

    svn_auth_get_ssl_server_trust_file_provider(&provider, apr_pool);
    APR_ARRAY_PUSH(providers, svn_auth_provider_object_t*) = provider;

    svn_auth_get_ssl_client_cert_file_provider(&provider, apr_pool);
    APR_ARRAY_PUSH(providers, svn_auth_provider_object_t*) = provider;

    svn_auth_get_ssl_client_cert_pw_file_provider2(&provider, nullptr, nullptr, apr_pool);
    APR_ARRAY_PUSH(providers, svn_auth_provider_object_t*) = provider;

    svn_auth_open(&ab, providers, apr_pool);
    ctx->auth_baton = ab;
}