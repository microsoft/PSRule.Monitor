// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PSRule.Monitor.Configuration;
using System;

namespace PSRule.Monitor.Pipeline
{
    internal sealed class PipelineContext
    {
        internal readonly PSRuleOption Option;

        // Track whether Dispose has been called.
        private bool _Disposed = false;

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
}
