// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace PSRule.Monitor;

internal sealed class LogEntry(string signature, DateTime date, string resourceId, string json)
{
    public string Signature { get; } = signature;

    public DateTime Date { get; } = date;

    public string ResourceId { get; } = resourceId;

    public string Json { get; } = json;
}
