// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PSRule.Monitor.Pipeline;
using System;
using System.Collections.Generic;

namespace PSRule.Monitor
{
    internal sealed class TestLogClient : ILogClient
    {
        public TestLogClient()
        {
            Output = new List<LogEntry>();
        }

        public List<LogEntry> Output { get; }

        public void Dispose()
        {
            // Test class only
        }

        public void Post(string signature, DateTime date, string resourceId, string json)
        {
            Output.Add(new LogEntry(
                signature,
                date,
                resourceId,
                json
            ));
        }
    }

    internal sealed class LogEntry
    {
        public LogEntry(string signature, DateTime date, string resourceId, string json)
        {
            Signature = signature;
            Date = date;
            ResourceId = resourceId;
            Json = json;
        }

        public string Signature { get; }

        public DateTime Date { get; }

        public string ResourceId { get; }

        public string Json { get; }
    }
}
