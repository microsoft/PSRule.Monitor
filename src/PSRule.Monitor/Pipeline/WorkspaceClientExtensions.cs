// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Management.Automation;

namespace PSRule.Monitor.Pipeline;

internal static class WorkspaceClientExtensions
{
    public static void Enqueue(this WorkspaceClient client, PSObject[] results)
    {
        if (results == null || results.Length == 0)
            return;

        for (var i = 0; i < results.Length; i++)
            client.Enqueue(results[i]);
    }
}
