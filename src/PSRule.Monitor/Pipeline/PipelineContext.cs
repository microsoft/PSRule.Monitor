// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PSRule.Monitor.Configuration;

namespace PSRule.Monitor.Pipeline;

internal sealed class PipelineContext : IDisposable
{
    internal readonly PSRuleOption Option;

    // Track whether Dispose has been called.
    private bool _Disposed;

    public PipelineContext(PSRuleOption option)
    {
        Option = option;
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
            {
                // Add cleanup
            }
            _Disposed = true;
        }
    }

    #endregion IDisposable
}
