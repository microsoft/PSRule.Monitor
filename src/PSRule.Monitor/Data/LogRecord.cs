// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace PSRule.Monitor.Data
{
    /// <summary>
    /// An Azure Monitor log record.
    /// </summary>
    internal sealed class LogRecord
    {
        public string RuleName { get; set; }

        public string TargetName { get; set; }

        public string TargetType { get; set; }

        public string Outcome { get; set; }

        public string ResourceId { get; set; }
    }
}
