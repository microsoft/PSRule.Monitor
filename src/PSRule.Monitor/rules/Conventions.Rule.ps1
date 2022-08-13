# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

# Synopsis: A convention to automatically ingest results into a Log Analysis workspace.
Export-PSRuleConvention 'Monitor.LogAnalytics.Import' -End {
    if ([String]::IsNullOrEmpty($Configuration.MONITOR_WORKSPACE_ID)) {
        Write-Error -Message ([PSRule.Monitor.Resources.PSRuleResources]::InvalidWorkspaceId) -ErrorId 'PSRule.Monitor.InvalidWorkspaceId';
        return;
    }
    if ([String]::IsNullOrEmpty($Configuration.MONITOR_WORKSPACE_KEY)) {
        Write-Error -Message ([PSRule.Monitor.Resources.PSRuleResources]::InvalidSharedKey) -ErrorId 'PSRule.Monitor.InvalidSharedKey';
        return;
    }

    $results = @($PSRule.Output | ForEach-Object {
        $_.AsRecord()
    })
    Write-Verbose -Message "[Monitor.LogAnalytics.Import] - Will ingest '$($results.Count)' results into Log Analytics.";
    [PSRule.Monitor.Runtime.Workspace]::Send($results, $Configuration.MONITOR_WORKSPACE_ID, $Configuration.MONITOR_WORKSPACE_KEY);
}
