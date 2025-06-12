// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Management.Automation;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PSRule.Monitor.Pipeline;

/// <summary>
/// A base class for all pipeline exceptions.
/// </summary>
public abstract class PipelineException : Exception
{
    protected PipelineException()
        : base() { }

    protected PipelineException(string message)
        : base(message) { }

    protected PipelineException(string message, Exception innerException)
        : base(message, innerException) { }

    protected PipelineException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}

/// <summary>
/// A base class for runtime exceptions.
/// </summary>
public abstract class RuntimeException : PipelineException
{
    protected RuntimeException()
        : base() { }

    protected RuntimeException(string message)
        : base(message) { }

    protected RuntimeException(string message, Exception innerException)
        : base(message, innerException) { }

    protected RuntimeException(Exception innerException, InvocationInfo invocationInfo, string ruleId)
        : base(innerException?.Message, innerException)
    {
        CommandInvocation = invocationInfo;
        RuleId = ruleId;
    }

    protected RuntimeException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }

    public InvocationInfo CommandInvocation { get; }

    public string RuleId { get; }
}

/// <summary>
/// An exception when building the pipeline.
/// </summary>
[Serializable]
public sealed class PipelineBuilderException : PipelineException
{
    /// <summary>
    /// Creates a pipeline builder exception.
    /// </summary>
    public PipelineBuilderException()
        : base() { }

    /// <summary>
    /// Creates a pipeline builder exception.
    /// </summary>
    /// <param name="message">The detail of the exception.</param>
    public PipelineBuilderException(string message)
        : base(message) { }

    /// <summary>
    /// Creates a pipeline builder exception.
    /// </summary>
    /// <param name="message">The detail of the exception.</param>
    /// <param name="innerException">A nested exception that caused the issue.</param>
    public PipelineBuilderException(string message, Exception innerException)
        : base(message, innerException) { }

    private PipelineBuilderException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        base.GetObjectData(info, context);
    }
}
