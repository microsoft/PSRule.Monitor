# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

#
# PSRule.Monitor module
#

Set-StrictMode -Version latest;

[PSRule.Monitor.Configuration.PSRuleOption]::UseExecutionContext($ExecutionContext);

#
# Localization
#

#
# Public functions
#

#region Public functions

# .ExternalHelp PSRule.Monitor-Help.xml
function Send-PSRuleMonitorRecord {
    [CmdletBinding()]
    [OutputType([void])]
    param (
        [Parameter(Mandatory = $True)]
        [String]$WorkspaceId,

        [Parameter(Mandatory = $True)]
        [SecureString]$SharedKey,

        [Parameter(Mandatory = $False, ValueFromPipeline = $True)]
        [PSObject]$InputObject
    )
    begin {
        Write-Verbose -Message '[Send-PSRuleMonitorRecord] BEGIN::';

        # Build the pipeline
        $builder = [PSRule.Monitor.Pipeline.PipelineBuilder]::Injest($Null);
        $builder.WorkspaceId($WorkspaceId);
        $builder.SharedKey($SharedKey);

        $builder.UseCommandRuntime($PSCmdlet.CommandRuntime);
        $builder.UseExecutionContext($ExecutionContext);
        try {
            $pipeline = $builder.Build();
            $pipeline.Begin();
        }
        catch {
            $pipeline.Dispose();
        }
    }
    process {
        if ($Null -ne (Get-Variable -Name pipeline -ErrorAction SilentlyContinue)) {
            try {
                $pipeline.Process($InputObject);
            }
            catch {
                $pipeline.Dispose();
                throw;
            }
        }
    }
    end {
        if ($Null -ne (Get-Variable -Name pipeline -ErrorAction SilentlyContinue)) {
            try {
                $pipeline.End();
            }
            finally {
                $pipeline.Dispose();
            }
        }
        Write-Verbose -Message '[Send-PSRuleMonitorRecord] END::';
    }
}

#endregion Public functions

#
# Helper functions
#

#
# Export module
#

Export-ModuleMember -Function Send-PSRuleMonitorRecord;
