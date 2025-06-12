// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using PSRule.Monitor.Pipeline;

namespace PSRule.Monitor;

internal sealed class TestLogClient : ILogClient
{
    public TestLogClient()
    {
        Output = [];
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
