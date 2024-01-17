// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Management.Automation;
using System.Security;
using System.Threading;
using PSRule.Monitor.Resources;

namespace PSRule.Monitor.Pipeline;

public interface IInjestPipelineBuilder : IPipelineBuilder
{
    void WorkspaceId(string workspaceId);

    void SharedKey(SecureString sharedKey);

    void LogName(string logName);
}

internal sealed class InjestPipelineBuilder : PipelineBuilderBase, IInjestPipelineBuilder
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
        return new InjestPipeline(PrepareContext(), PrepareReader(), _WorkspaceId, _SharedKey, logClient);
    }
}

internal sealed class InjestPipeline : PipelineBase
{
    private readonly WorkspaceClient _WorkspaceClient;

    // Track whether Dispose has been called.
    private bool _Disposed;

    internal InjestPipeline(PipelineContext context, PipelineReader reader, string workspaceId, SecureString sharedKey, ILogClient logClient)
        : base(context, reader)
    {
        _WorkspaceClient = new WorkspaceClient(workspaceId, sharedKey, logClient);
    }

    public override void Process(PSObject sourceObject)
    {
        Reader.Enqueue(sourceObject);
        while (Reader.TryDequeue(out var next))
            _WorkspaceClient.Enqueue(next);

        _WorkspaceClient.Send(30, 100);
    }

    public override void End()
    {
        _WorkspaceClient.Send();
    }

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
                _WorkspaceClient.Dispose();

            _Disposed = true;
        }
        base.Dispose(disposing);
    }

    #endregion IDisposable
}
