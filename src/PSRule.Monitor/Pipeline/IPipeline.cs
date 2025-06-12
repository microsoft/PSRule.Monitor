// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;

namespace PSRule.Monitor.Pipeline;

public interface IPipeline
{
    void Begin();

    void Process(PSObject sourceObject);

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords")]
    void End();
}
