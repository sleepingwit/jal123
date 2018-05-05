#include "stdafx.h"
#include "LogCmd.h"
#include <string>

using namespace System::Runtime::InteropServices;
using namespace BridgeCli;

struct LogMessage
{
    svn_revnum_t revision;
    std::string author;
    std::string message;
    friend bool operator<(const LogMessage& lhs, const LogMessage& rhs);
};

bool operator<(const LogMessage& lhs, const LogMessage& rhs)
{
    return lhs.revision < rhs.revision;
}

struct LogChangedPaths2Item
{
    std::string path;
    char action;
    std::string copyfrom_path;
    svn_revnum_t copyfrom_rev;
    svn_node_kind_t node_kind;
    svn_tristate_t text_modified;
    svn_tristate_t props_modified;
};

using NativeResultMap = std::map<LogMessage, std::vector<std::shared_ptr<LogChangedPaths2Item>>>;

struct LogReceiverBaton
{
    svn_client_ctx_t* ctx;
    const char* target_path_or_url;
    svn_opt_revision_t target_peg_revision;
    svn_depth_t depth;
    apr_pool_t* pool;
    NativeResultMap* native_result;
};

svn_error_t* LogEntryReceiver(void* baton, svn_log_entry_t* log_entry, apr_pool_t* pool);

LogCmdResult^ ParseClrResult(const NativeResultMap* nativeResult)
{
    LogCmdResult^ result = gcnew LogCmdResult();
    for (auto itKey = nativeResult->cbegin(); itKey != nativeResult->cend(); ++itKey)
    {
        LogCmdResultRevisionItem^ revisionItem = gcnew LogCmdResultRevisionItem();
        const LogMessage& logMessage = itKey->first;

        revisionItem->author = SvnCmd::GetUtf8Array(logMessage.author.c_str(), logMessage.author.size());
        revisionItem->message = SvnCmd::GetUtf8Array(logMessage.message.c_str(), logMessage.message.size());
        revisionItem->revision = logMessage.revision;

        List<LogCmdResultPathItem^>^ list = gcnew List<LogCmdResultPathItem^>();
        const std::vector<std::shared_ptr<LogChangedPaths2Item>>& vec = itKey->second;
        for (auto itLog = vec.cbegin(); itLog != vec.cend(); ++itLog)
        {
            LogCmdResultPathItem^ item = gcnew LogCmdResultPathItem();
            item->Path = SvnCmd::GetUtf8Array((*itLog)->path.c_str(), (*itLog)->path.size());

            char action = (*itLog)->action;
            switch(action)
            {
            case 'A':
                item->Action = LogItemAction::Add;
                break;
            case 'D':
                item->Action = LogItemAction::Delete;
                break;
            case 'R':
                item->Action = LogItemAction::Repalce;
                break;
            case 'M':
                item->Action = LogItemAction::Modifiy;
                break;
            default:
                throw gcnew Exception("Unknown LogItemAction");
            }

            item->CopyFromPath = SvnCmd::GetUtf8Array((*itLog)->copyfrom_path.c_str(), (*itLog)->copyfrom_path.size());
            item->CopyFromRev = (*itLog)->copyfrom_rev;

            switch((*itLog)->node_kind)
            {
            case svn_node_none:
                item->NodeKind = SvnNodeKind::None;
                break;
            case svn_node_file:
                item->NodeKind = SvnNodeKind::File;
                break;
            case svn_node_dir:
                item->NodeKind = SvnNodeKind::Dir;
                break;
            case svn_node_symlink:
                item->NodeKind = SvnNodeKind::Symlink;
                break;
            case svn_node_unknown:
            default:
                item->NodeKind = SvnNodeKind::Unknown;
                break;
            }

            list->Add(item);
        }

        result->Results->Add(revisionItem, list);
    }
    return result;
}

svn_error_t* LogEntryReceiver(void* baton, svn_log_entry_t* log_entry, apr_pool_t* pool)
{
    LogReceiverBaton* lb = reinterpret_cast<LogReceiverBaton*>(baton);
    NativeResultMap* result = reinterpret_cast<NativeResultMap*>(lb->native_result);
    svn_string_t* author = nullptr;
    svn_string_t* date = nullptr;
    svn_string_t* message = nullptr;

    if (log_entry->revprops)
    {
        author = reinterpret_cast<svn_string_t*>(svn_hash_gets(log_entry->revprops, SVN_PROP_REVISION_AUTHOR));
        date = reinterpret_cast<svn_string_t*>(svn_hash_gets(log_entry->revprops, SVN_PROP_REVISION_DATE));
        message = reinterpret_cast<svn_string_t*>(svn_hash_gets(log_entry->revprops, SVN_PROP_REVISION_LOG));
    }

    if (log_entry->revision == 0 && message == nullptr)
        return SVN_NO_ERROR;

    if (!SVN_IS_VALID_REVNUM(log_entry->revision))
    {
        return SVN_NO_ERROR;
    }

    LogMessage logMessage;
    logMessage.revision = log_entry->revision;
    logMessage.author.assign(author->data, author->len);
    logMessage.message.assign(message->data, message->len);

    if (log_entry->changed_paths2)
    {
        std::vector<std::shared_ptr<LogChangedPaths2Item>> vec;

        apr_hash_index_t* hi = apr_hash_first(pool, log_entry->changed_paths2);
        for (; hi; hi = apr_hash_next(hi))
        {
            std::shared_ptr<LogChangedPaths2Item> item = std::make_shared<LogChangedPaths2Item>();

            const void* path = nullptr;
            apr_ssize_t klen = 0;
            void* val = nullptr;
            apr_hash_this(hi, &path, &klen, &val);
			const char* decoded = svn_path_uri_decode((const char*)path, pool);

            item->path.assign(decoded);

            svn_log_changed_path2_t* internalItem = (svn_log_changed_path2_t*)val;

            item->action = internalItem->action;
            if (internalItem->copyfrom_path && SVN_IS_VALID_REVNUM(internalItem->copyfrom_rev))
            {
                item->copyfrom_path.assign(internalItem->copyfrom_path);
                item->copyfrom_rev = internalItem->copyfrom_rev;
            }
            else
            {
                item->copyfrom_path.clear();
                item->copyfrom_rev = 0;
            }

            item->node_kind = internalItem->node_kind;
            item->text_modified = internalItem->text_modified;
            item->props_modified = internalItem->props_modified;

            vec.push_back(item);
        }

        result->insert(std::make_pair(logMessage, vec));
    }
    return SVN_NO_ERROR;
}

LogCmdResult^ LogCmd::Run(ICollection<IntPtr>^ urls, ICollection<unsigned int>^ revisions, BridgeCliOpt^ opt)
{
    if (urls->Count <= 0)
    {
        throw gcnew Exception("Empty Urls");
    }

    Authenticate();
    
    opt->revision_ranges = apr_array_make(apr_pool, 0, sizeof(svn_opt_revision_range_t*));

    for each(unsigned int changeno in revisions)
    {
        unsigned int changeno_end = changeno;
        //if (changeno > 0)
        //{
        //    changeno--;
        //}
        svn_opt_revision_range_t* range = static_cast<svn_opt_revision_range_t*>(apr_palloc(apr_pool, sizeof(*range)));
        range->start.kind = svn_opt_revision_number;
        range->start.value.number = changeno;
        range->end.kind = svn_opt_revision_number;
        range->end.value.number = changeno_end;

        APR_ARRAY_PUSH(opt->revision_ranges, svn_opt_revision_range_t*) = range;
    }

    opt->start_revision = &(APR_ARRAY_IDX(opt->revision_ranges, 0, svn_opt_revision_range_t*)->start);
    opt->end_revision = &(APR_ARRAY_IDX(opt->revision_ranges, 0, svn_opt_revision_range_t*)->end);

    EnsureRevisionRanges(opt);
    if (opt->depth == svn_depth_unknown)
    {
        opt->depth = svn_depth_infinity;
    }

    apr_array_header_t* targets = nullptr;
    ParseTargets(&targets, urls);

    const char* target = APR_ARRAY_IDX(targets, 0, const char*);
    LogReceiverBaton* lb =
        static_cast<LogReceiverBaton*>(apr_palloc(apr_pool, sizeof(LogReceiverBaton)));
    SVNERROR(svn_opt_parse_path(&lb->target_peg_revision, &lb->target_path_or_url, target, apr_pool));
    if (lb->target_peg_revision.kind == svn_opt_revision_unspecified)
    {
        lb->target_peg_revision.kind = (svn_path_is_url(target) ? svn_opt_revision_head : svn_opt_revision_working);
    }
    APR_ARRAY_IDX(targets, 0, const char*) = lb->target_path_or_url;

    if (svn_path_is_url(target))
    {
        for (int i = 1; i < targets->nelts; i++)
        {
            target = APR_ARRAY_IDX(targets, i, const char*);
            if (svn_path_is_url(target) || target[0] == '/')
            {
                char temp[1024] = { 0 };
                snprintf(temp, sizeof(temp), "Only relative paths can be specified"
                    " after a URL for 'svn log', "
                    "but '%s' is not a relative path", target);
                throw gcnew Exception(gcnew String(temp));
            }
        }
    }

    lb->ctx = ctx;
    lb->depth = (svn_depth_t)opt->depth;
    lb->pool = apr_pool;
    lb->native_result = new NativeResultMap;

    apr_array_header_t* revprops = apr_array_make(apr_pool, 3, sizeof(char*));
    APR_ARRAY_PUSH(revprops, const char*) = SVN_PROP_REVISION_AUTHOR;
    APR_ARRAY_PUSH(revprops, const char*) = SVN_PROP_REVISION_DATE;
    APR_ARRAY_PUSH(revprops, const char*) = SVN_PROP_REVISION_LOG;

    SVNERROR(svn_client_log5(targets,
        &lb->target_peg_revision,
        opt->revision_ranges,
        opt->limit,
        opt->verbose,
        opt->stop_on_copy,
        opt->use_merge_history,
        revprops,
        LogEntryReceiver,
        lb,
        ctx,
        apr_pool));

    LogCmdResult^ clrResult = ParseClrResult(lb->native_result);
    delete lb->native_result;

    return clrResult;
}

void LogCmd::ParseTargets(apr_array_header_t** targets_p, ICollection<IntPtr>^ urls)
{
    apr_getopt_t aprOpt;
    aprOpt.argc = 0;

    apr_array_header_t* known_targets
        = apr_array_make(apr_pool, 0, sizeof(const char*));

    for each(IntPtr url in urls)
    {
        APR_ARRAY_PUSH(known_targets, const char*) =
            static_cast<const char*>(url.ToPointer());
    }

    SVNERROR(svn_client_args_to_target_array2(targets_p, &aprOpt, known_targets, ctx, FALSE, apr_pool));
}
