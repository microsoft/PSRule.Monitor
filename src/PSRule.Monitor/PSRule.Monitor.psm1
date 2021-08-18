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
        [Parameter(Mandatory = $False)]
        [String]$WorkspaceId = $Env:PSRULE_CONFIGURATION_MONITOR_WORKSPACE_ID,

        [Parameter(Mandatory = $False)]
        [PSRule.Monitor.SecureStringAttribute()]
        [SecureString]$SharedKey = $Env:PSRULE_CONFIGURATION_MONITOR_WORKSPACE_KEY,

        [Parameter(Mandatory = $False, ValueFromPipeline = $True)]
        [PSObject]$InputObject,

        [Parameter(Mandatory = $False)]
        [PSDefaultValue(Value = 'PSRule')]
        [String]$LogName
    )
    begin {
        Write-Verbose -Message '[Send-PSRuleMonitorRecord] BEGIN::';
        $pipelineReady = $False;

        try {
            # Build the pipeline
            $builder = [PSRule.Monitor.Pipeline.PipelineBuilder]::Injest($Null);
            $builder.WorkspaceId($WorkspaceId);
            $builder.SharedKey($SharedKey);

            if ($PSBoundParameters.ContainsKey('LogName')) {
                $builder.LogName($LogName);
            }

            $builder.UseCommandRuntime($PSCmdlet.CommandRuntime);
            $builder.UseExecutionContext($ExecutionContext);

            $pipeline = $builder.Build();
            if ($Null -ne $pipeline) {
                $pipeline.Begin();
                $pipelineReady = $True;
            }
        }
        catch {
            throw $_.Exception.GetBaseException();
        }
    }
    process {
        if ($pipelineReady) {
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
        if ($pipelineReady) {
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
