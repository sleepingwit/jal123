#include "stdafx.h"
#include "ExportCmd.h"

using namespace System;
using namespace BridgeCli;
using namespace System::Runtime::InteropServices;

void ExportCmd::Run(IntPtr fileUrlAtPegRevision, IntPtr destPath, BridgeCliOpt^ opt)
{
    this->Run(fileUrlAtPegRevision, destPath, opt, 0);
}

void ExportCmd::Run(IntPtr fileUrlAtPegRevision, IntPtr destPath, BridgeCliOpt^ opt, unsigned int revision)
{
    const char* source = reinterpret_cast<const char*>(fileUrlAtPegRevision.ToPointer());
    const char* dest = reinterpret_cast<const char*>(destPath.ToPointer());

    if (revision > 0)
    {
        opt->revision_ranges = apr_array_make(apr_pool, 0, sizeof(svn_opt_revision_range_t*));

        svn_opt_revision_range_t* range = static_cast<svn_opt_revision_range_t*>(apr_palloc(apr_pool, sizeof(*range)));
        range->start.kind = svn_opt_revision_number;
        range->start.value.number = revision;
        range->end.kind = svn_opt_revision_number;
        range->end.value.number = revision;
        APR_ARRAY_PUSH(opt->revision_ranges, svn_opt_revision_range_t*) = range;

        opt->start_revision = &(APR_ARRAY_IDX(opt->revision_ranges, 0, svn_opt_revision_range_t*)->start);
        opt->end_revision = &(APR_ARRAY_IDX(opt->revision_ranges, 0, svn_opt_revision_range_t*)->end);
    }
    Authenticate();

    EnsureRevisionRanges(opt);

    svn_opt_revision_t peg_revision;
    const char* truefrom = nullptr;
    const char* to = dest;

	SVNERROR(svn_opt_parse_path(&peg_revision, &truefrom, source, apr_pool));

    if(svn_path_is_url(to))
    {
        throw gcnew Exception(gcnew String("Dest path is url"));
    }

    if (opt->depth == svn_depth_unknown)
    {
        opt->depth = svn_depth_infinity;
    }

    SVNERROR(svn_client_export5(nullptr, truefrom, to, &peg_revision, opt->start_revision,
        opt->force, opt->ignore_externals, opt->ignore_keywords, (svn_depth_t)opt->depth, nullptr, ctx, apr_pool));
}
