// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PSRule.Monitor.Pipeline;
using System.Management.Automation;
using System.Net;
using System.Security;

namespace PSRule.Monitor.Runtime
{
    /// <summary>
    /// Helper methods exposed to PowerShell for interacting with a Log Analytics workspace.
    /// </summary>
    public static class Workspace
    {
        public static void Send(PSObject[] results, string workspaceId, string sharedKey)
        {
            var logClient = new LogClient(workspaceId, "PSRule");
            using (var client = new WorkspaceClient(workspaceId, GetSecureString(sharedKey), logClient))
            {
                client.Enqueue(results);
                client.Send();
            }
        }

        private static SecureString GetSecureString(string sharedKey)
        {
            return new NetworkCredential("na", sharedKey).SecurePassword;
        }
    }
}
