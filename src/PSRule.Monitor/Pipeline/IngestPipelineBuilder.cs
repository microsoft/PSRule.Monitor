// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security;
using PSRule.Monitor.Resources;

namespace PSRule.Monitor.Pipeline;

internal sealed class IngestPipelineBuilder : PipelineBuilderBase, IIngestPipelineBuilder
{
    private const string DEFAULT_LOGNAME = "PSRule";

    private string _WorkspaceId;
    private SecureString _SharedKey;
    private string _LogName = DEFAULT_LOGNAME;

    public void WorkspaceId(string workspaceId)
    {
        if (string.IsNullOrEmpty(workspaceId))
            throw new PipelineBuilderException(string.Format(Thread.CurrentThread.CurrentCulture, PSRuleResources.InvalidWorkspaceId));

        _WorkspaceId = workspaceId;
    }

    public void SharedKey(SecureString sharedKey)
    {
        if (sharedKey == null)
            throw new PipelineBuilderException(string.Format(Thread.CurrentThread.CurrentCulture, PSRuleResources.InvalidSharedKey));

        _SharedKey = sharedKey;
    }

    public void LogName(string logName)
    {
        _LogName = logName;
    }

    public override IPipeline Build()
    {
        var logClient = new LogClient(_WorkspaceId, _LogName);
        return new IngestPipeline(PrepareContext(), PrepareReader(), _WorkspaceId, _SharedKey, logClient);
    }
}
