# PSRule for Azure Monitor

Log PSRule analysis results to Azure Monitor.

> [!NOTE]
> This module is being migrated into <https://github.com/microsoft/PSRule>
> and this repository will be archived in the future.
> The functionality of this module will be included PSRule in a future release.
> This repository currently reflects functionality as a standalone module.
> Please log any new issues at <https://github.com/microsoft/PSRule/issues>
---

## Support

This project uses GitHub Issues to track bugs and feature requests.
Please search the existing issues before filing new issues to avoid duplicates.

- For new issues, file your bug or feature request as a new [issue].

Support for this project/ product is limited to the resources listed above.

## Getting the module

This project requires the `PSRule` PowerShell module.
For detail on installation and dependencies see [install].

You can download and install these modules from the PowerShell Gallery.

Module             | Description | Downloads / instructions
------             | ----------- | ------------------------
PSRule.Monitor     | Log PSRule analysis results to Azure Monitor. | [latest][module] / [instructions][install]

## Getting started

### Upload results with PSRule convention

A convention can be used to upload the results of a PSRule analysis to Azure Monitor.
To use the convention:

- Install the `PSRule.Monitor` module from the PowerShell Gallery.
- Include the `PSRule.Monitor` module.
  This can be set in PSRule options or specified at runtime as a parameter.
- Reference `Monitor.LogAnalytics.Import` convention.
  This can be set in PSRule options or specified at runtime as a parameter.

For example:

```yaml
include:
  module:
  - 'PSRule.Monitor'

convention:
  include:
  - 'Monitor.LogAnalytics.Import
```

When using the convention a Log Analytics workspace must be specified.
This can be done by setting the following environment variables:

- `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_ID` - Containing the Log Analytics workspace ID.
- `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_KEY` - Containing either the primary or secondary key to the workspace.
  This value is sensitive and should be stored securely.
  To protect this value, avoid storing it in source control.

For example:

```powershell
# PowerShell: Setting environment variable
$Env:PSRULE_CONFIGURATION_MONITOR_WORKSPACE_ID = '00000000-0000-0000-0000-000000000000'
$Env:PSRULE_CONFIGURATION_MONITOR_WORKSPACE_KEY = Get-Secret -Name 'WORKSPACE_KEY' -AsPlainText
```

```yaml
# GitHub Actions: Setting environment variable with microsoft/ps-rule action
- name: Run PSRule analysis
  uses: microsoft/ps-rule@main
  env:
    PSRULE_CONFIGURATION_MONITOR_WORKSPACE_ID: 00000000-0000-0000-0000-000000000000
    PSRULE_CONFIGURATION_MONITOR_WORKSPACE_KEY: ${{ secrets.WORKSPACE_KEY }}
```

```yaml
# Azure Pipelines: Setting environment variable with ps-rule-assert task
- task: ps-rule-assert@0
  displayName: Run PSRule analysis
  inputs:
    inputType: repository
  env:
    PSRULE_CONFIGURATION_MONITOR_WORKSPACE_ID: 00000000-0000-0000-0000-000000000000
    PSRULE_CONFIGURATION_MONITOR_WORKSPACE_KEY: $(WORKSPACE_KEY)
```

### Upload results with PowerShell

To upload results from PSRule to Azure Monitor, use the `Send-PSRuleMonitorRecord` cmdlet.
Results can by piped directly from `Invoke-PSRule` or stored and piped from a variable.

The `Send-PSRuleMonitorRecord` cmdlet requires a Log Analytics workspace to send data to.
A workspace can be specified by using the `-WorkspaceId` and `-SharedKey` parameters.

For example:

```powershell
$data | Invoke-PSRule | Send-PSRuleMonitorRecord -WorkspaceId <workspaceId> -SharedKey <primaryKey>;
```

The following example shows using analysis results from a pre-built module:

```powershell
$results = Invoke-PSRule -InputPath .\*.json -Module 'PSRule.Rules.Azure';
$results | Send-PSRuleMonitorRecord -WorkspaceId <workspaceId> -SharedKey <primaryKey>;
```

### Querying logs from Azure Monitor

By default, PSRule results are stored in the `PSRule_CL` table.
The results can be queries from the Log Analytics workspace using Kusto.

The following query returns all rule records from the last hour that failed:

```kusto
PSRule_CL
| where Outcome_s == "Fail" and TimeGenerated > ago(1h)
```

To query these results from PowerShell use:

```powershell
Invoke-AzOperationalInsightsQuery -WorkspaceId <workspaceId> -Query 'PSRule_CL | where Outcome_s == "Fail" and TimeGenerated > ago(1h)'
```

## Language reference

### Commands

The following commands exist in the `PSRule.Monitor` module:

- [Send-PSRuleMonitorRecord](docs/commands/PSRule.Monitor/en-US/Send-PSRuleMonitorRecord.md) - Send analysis results from PSRule to Azure Monitor.

## Contributing

This project welcomes contributions and suggestions.
If you are ready to contribute, please visit the [contribution guide](CONTRIBUTING.md).

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Maintainers

- [Bernie White](https://github.com/BernieWhite)

## License

This project is [licensed under the MIT License](LICENSE).

[install]: docs/scenarios/install-instructions.md
[module]: https://www.powershellgallery.com/packages/PSRule.Monitor
[issue]: https://github.com/microsoft/PSRule/issues
