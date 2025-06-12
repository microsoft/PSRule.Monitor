// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PSRule.Monitor.Configuration;

namespace PSRule.Monitor.Pipeline;

public static class PipelineBuilder
{
    public static IIngestPipelineBuilder Ingest(PSRuleOption option)
    {
        var builder = new IngestPipelineBuilder();
        builder.Configure(option);
        return builder;
    }
}
