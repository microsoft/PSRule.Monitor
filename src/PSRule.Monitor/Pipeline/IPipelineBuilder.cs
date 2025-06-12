// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using PSRule.Monitor.Configuration;

namespace PSRule.Monitor.Pipeline;

public interface IPipelineBuilder
{
    void UseCommandRuntime(ICommandRuntime2 commandRuntime);

    void UseExecutionContext(EngineIntrinsics executionContext);

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords")]
    IPipelineBuilder Configure(PSRuleOption option);

    IPipeline Build();
}
