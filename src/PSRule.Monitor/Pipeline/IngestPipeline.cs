// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Management.Automation;
using System.Security;

namespace PSRule.Monitor.Pipeline;

internal sealed class IngestPipeline : PipelineBase
{
    private readonly WorkspaceClient _WorkspaceClient;

    // Track whether Dispose has been called.
    private bool _Disposed;

    internal IngestPipeline(PipelineContext context, PipelineReader reader, string workspaceId, SecureString sharedKey, ILogClient logClient)
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
