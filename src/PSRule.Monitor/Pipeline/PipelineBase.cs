// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Management.Automation;

namespace PSRule.Monitor.Pipeline;

internal abstract class PipelineBase : IDisposable, IPipeline
{
    protected readonly PipelineContext Context;
    protected readonly PipelineReader Reader;

    // Track whether Dispose has been called.
    private bool _Disposed;

    protected PipelineBase(PipelineContext context, PipelineReader reader)
    {
        Context = context;
        Reader = reader;
    }

    #region IPipeline

    public virtual void Begin()
    {
        // Do nothing
    }

    public virtual void Process(PSObject sourceObject)
    {
        // Do nothing
    }

    public virtual void End()
    {
        // Do nothing
    }

    #endregion IPipeline

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            _Disposed = true;
        }
    }

    #endregion IDisposable
}
