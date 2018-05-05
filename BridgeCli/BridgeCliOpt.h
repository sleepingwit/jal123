#pragma once

namespace BridgeCli
{
    ref struct BridgeCliOpt sealed
    {
        BridgeCliOpt()
        {
            revision_ranges = nullptr;
            start_revision = nullptr;
            end_revision = nullptr;
            depth = svn_depth_unknown;
            set_depth = svn_depth_unknown;
        }

        int depth;
        int set_depth;
        int limit;
        svn_boolean_t verbose;
        svn_boolean_t stop_on_copy;
        svn_boolean_t use_merge_history;
        svn_boolean_t force;
        svn_boolean_t ignore_externals;
        svn_boolean_t ignore_keywords;

        apr_array_header_t * revision_ranges;
        svn_opt_revision_t* start_revision;
        svn_opt_revision_t* end_revision;
    };

}
