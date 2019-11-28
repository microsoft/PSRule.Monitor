// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Management.Automation;

namespace PSRule.Monitor.Pipeline
{
    internal sealed class PipelineReader
    {
        private readonly ConcurrentQueue<PSObject> _Queue;

        public PipelineReader()
        {
            _Queue = new ConcurrentQueue<PSObject>();
        }

        public int Count
        {
            get { return _Queue.Count; }
        }

        public bool IsEmpty
        {
            get { return _Queue.IsEmpty; }
        }

        public void Enqueue(PSObject sourceObject)
        {
            if (sourceObject == null)
                return;

            _Queue.Enqueue(sourceObject);
        }

        public bool TryDequeue(out PSObject sourceObject)
        {
            return _Queue.TryDequeue(out sourceObject);
        }
    }
}
