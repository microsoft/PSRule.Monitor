// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace PSRule.Monitor.Pipeline;

internal interface ILogClient : IDisposable
{
    void Post(string signature, DateTime date, string resourceId, string json);
}
