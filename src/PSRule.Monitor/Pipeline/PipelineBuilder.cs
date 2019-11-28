// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Management.Automation;

namespace PSRule.Monitor.Pipeline
{
    public static class PipelineBuilder
    {
        public static IInjestPipelineBuilder Injest(PSRuleOption option)
        {
            var builder = new InjestPipelineBuilder();
            builder.Configure(option);
            return builder;
        }
    }

    public interface IPipelineBuilder
    {
        void UseCommandRuntime(ICommandRuntime2 commandRuntime);

        void UseExecutionContext(EngineIntrinsics executionContext);

        IPipelineBuilder Configure(PSRuleOption option);

        IPipeline Build();
    }

    public interface IPipeline
    {
        void Begin();

        void Process(PSObject sourceObject);

        void End();
    }

    internal abstract class PipelineBuilderBase : IPipelineBuilder
    {
        protected readonly PSRuleOption Option;

        protected PipelineBuilderBase()
        {
            Option = new PSRuleOption();
        }

        public virtual void UseCommandRuntime(ICommandRuntime2 commandRuntime)
        {
            //Logger.UseCommandRuntime(commandRuntime);
        }

        public void UseExecutionContext(EngineIntrinsics executionContext)
        {
            //Logger.UseExecutionContext(executionContext);
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

    internal abstract class PipelineBase : IDisposable, IPipeline
    {
        protected readonly PipelineContext Context;
        protected readonly PipelineReader Reader;

        // Track whether Dispose has been called.
        private bool _Disposed = false;

        protected PipelineBase(PipelineContext context, PipelineReader reader)
        {
            Context = context;
            Reader = reader;
        }

        #region IPipeline

        public virtual void Begin()
        {
            // Reader.Open();
        }

        public virtual void Process(PSObject sourceObject)
        {
            // Do nothing
        }

        public virtual void End()
        {
            //Writer.End();
        }

        #endregion IPipeline

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
                _Disposed = true;
            }
        }

        #endregion IDisposable
    }
}
