// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Newtonsoft.Json;

namespace PSRule.Monitor.Data;

/// <summary>
/// An Azure Monitor log record.
/// </summary>
internal sealed class LogRecord
{
    public string RuleId { get; set; }

    public string RuleName { get; set; }

    public string DisplayName { get; set; }

    public string ModuleName { get; set; }

    public string TargetName { get; set; }

    public string TargetType { get; set; }

    public string Outcome { get; set; }

    [JsonIgnore]
    public string ResourceId { get; set; }

    [JsonConverter(typeof(StringifyMapConverter))]
    public Hashtable Field { get; set; }

    [JsonConverter(typeof(StringifyMapConverter))]
    public Hashtable Data { get; set; }

    [JsonConverter(typeof(StringifyMapConverter))]
    public Hashtable Annotations { get; set; }

    public string RunId { get; set; }

    public Guid CorrelationId { get; set; }

    public long Duration { get; set; }
}
