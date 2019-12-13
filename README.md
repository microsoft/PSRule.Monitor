# PSRule to Azure Monitor

Log PSRule analysis results to Azure Monitor.

![ci-badge]

**More to come soon.**

## Disclaimer

This project is to be considered a **proof-of-concept** and **not a supported product**.

If you have any problems please check our GitHub [issues](https://github.com/Microsoft/PSRule.Monitor/issues) page.
If you do not see your problem captured, please file a new issue and follow the provided template.

If you have any problems with the [PSRule][engine] engine, please check the project GitHub [issues](https://github.com/Microsoft/PSRule/issues) page instead.

## Getting started

### Upload results

To upload results from PSRule to Azure Monitor, use the `Send-PSRuleMonitorRecord` cmdlet.
Results can by piped directly from `Invoke-PSRule` or stored and piped from a variable.

The `Send-PSRuleMonitorRecord` requires a Log Analytics workspace to send data to.
The Id and key of the workspace can be specified by `-WorkspaceId` and `-SharedKey`.

For example:

```powershell
$data | Invoke-PSRule | Send-PSRuleMonitorRecord;
```

The following example shows using analysis results from a pre-built module:

```powershell
$results = Invoke-PSRule -InputPath .\*.json -Module 'PSRule.Rules.Azure';
$results | Send-PSRuleMonitorRecord -WorkspaceId <workspaceId> -SharedKey <primaryKey>;
```

### Querying logs from Azure Monitor

The following query reviews all rule records from the last hour that failed:

```text
PSRule_CL
| where Outcome_s == "Fail" and TimeGenerated > ago(1h)
```

## Language reference

### Commands

The following commands exist in the `PSRule.Monitor` module:

- [Send-PSRuleMonitorRecord](docs/commands/PSRule.Monitor/en-US/Send-PSRuleMonitorRecord.md) - Send analysis results from PSRule to Azure Monitor.

## Changes and versioning

Modules in this repository will use the [semantic versioning](http://semver.org/) model to declare breaking changes from v1.0.0.
Prior to v1.0.0, breaking changes may be introduced in minor (0.x.0) version increments.
For a list of module changes please see the [change log](CHANGELOG.md).

> Pre-release module versions are created on major commits and can be installed from the PowerShell Gallery.
> Pre-release versions should be considered experimental.
> Modules and change log details for pre-releases will be removed as standard releases are made available.

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

[ci-badge]: https://dev.azure.com/bewhite/PSRule.Monitor/_apis/build/status/PSRule.Monitor-CI?branchName=master
[module-psrule]: https://www.powershellgallery.com/packages/PSRule.Monitor
