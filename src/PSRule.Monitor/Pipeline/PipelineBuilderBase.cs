// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Management.Automation;
using PSRule.Monitor.Configuration;

namespace PSRule.Monitor.Pipeline;

internal abstract class PipelineBuilderBase : IPipelineBuilder
{
    protected readonly PSRuleOption Option;

    protected PipelineBuilderBase()
    {
        Option = new PSRuleOption();
    }

    public virtual void UseCommandRuntime(ICommandRuntime2 commandRuntime)
    {
        // Do nothing
    }

    public void UseExecutionContext(EngineIntrinsics executionContext)
    {
        // Do nothing
    }

    public virtual IPipelineBuilder Configure(PSRuleOption option)
    {
        return this;
    }

    public abstract IPipeline Build();

    protected PipelineContext PrepareContext()
    {
        return new PipelineContext(Option);
    }

    protected virtual PipelineReader PrepareReader()
    {
        return new PipelineReader();
    }
}
