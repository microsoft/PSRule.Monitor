// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security;

namespace PSRule.Monitor.Pipeline;

public interface IIngestPipelineBuilder : IPipelineBuilder
{
    void WorkspaceId(string workspaceId);

    void SharedKey(SecureString sharedKey);

    void LogName(string logName);
}
