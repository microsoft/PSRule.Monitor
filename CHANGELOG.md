# Change log

## Unreleased

What's changed since v0.5.0:

- Engineering:
  - Bump Microsoft.NET.Test.Sdk to v17.3.2.
    [#98](https://github.com/microsoft/PSRule.Monitor/pull/98)
  - Bump PSRule to v2.7.0.
    [#95](https://github.com/microsoft/PSRule.Monitor/pull/95)
  - Bump PSScriptAnalyzer to v1.21.0.
    [#95](https://github.com/microsoft/PSRule.Monitor/pull/95)
  - Bump coverlet.collector to v3.2.0.
    [#106](https://github.com/microsoft/PSRule.Monitor/pull/106)

## v0.5.0

What's changed since v0.4.0:

- Engineering:
  - Bump PSRule to v2.3.2.
    [#90](https://github.com/microsoft/PSRule.Monitor/pull/90)
  - Bump Microsoft.PowerShell.SDK to v7.2.6.
    [#80](https://github.com/microsoft/PSRule.Monitor/pull/80)
  - Bump Microsoft.NET.Test.Sdk to v17.3.0.
    [#79](https://github.com/microsoft/PSRule.Monitor/pull/79)
  - Upgrade support projects to .NET 6 by @BernieWhite.
    [#83](https://github.com/microsoft/PSRule.Monitor/issues/83)
  - Automatically update PowerShell dependencies by @BernieWhite.
    [#84](https://github.com/microsoft/PSRule.Monitor/issues/84)
  - Update Pester tests to v5 by @BernieWhite.
    [#15](https://github.com/microsoft/PSRule.Monitor/issues/15)
  - Added code signing and SBOM metadata by @BernieWhite.
    [#86](https://github.com/microsoft/PSRule.Monitor/issues/86)

What's changed since pre-release v0.5.0-B0014:

- No additional changes.

## v0.5.0-B0014 (pre-release)

What's changed since pre-release v0.5.0-B0005:

- Engineering:
  - Bump PSRule to v2.3.2.
    [#90](https://github.com/microsoft/PSRule.Monitor/pull/90)

## v0.5.0-B0005 (pre-release)

What's changed since v0.4.0:

- Engineering:
  - Bump Microsoft.PowerShell.SDK to v7.2.6.
    [#80](https://github.com/microsoft/PSRule.Monitor/pull/80)
  - Bump Microsoft.NET.Test.Sdk to v17.3.0.
    [#79](https://github.com/microsoft/PSRule.Monitor/pull/79)
  - Upgrade support projects to .NET 6 by @BernieWhite.
    [#83](https://github.com/microsoft/PSRule.Monitor/issues/83)
  - Automatically update PowerShell dependencies by @BernieWhite.
    [#84](https://github.com/microsoft/PSRule.Monitor/issues/84)
  - Update Pester tests to v5 by @BernieWhite.
    [#15](https://github.com/microsoft/PSRule.Monitor/issues/15)
  - Added code signing and SBOM metadata by @BernieWhite.
    [#86](https://github.com/microsoft/PSRule.Monitor/issues/86)

## v0.4.0

What's changed since v0.3.0:

- Engineering:
  - Bump Newtonsoft.Json to v13.0.1.
    [#73](https://github.com/microsoft/PSRule.Monitor/pull/73)
  - Bump xunit to v2.4.2.
    [#76](https://github.com/microsoft/PSRule.Monitor/pull/76)

What's changed since pre-release v0.4.0-B2208003:

- No additional changes.

## v0.4.0-B2208003 (pre-release)

What's changed since pre-release v0.4.0-B2206007:

- Engineering:
  - Bump xunit to v2.4.2.
    [#76](https://github.com/microsoft/PSRule.Monitor/pull/76)

## v0.4.0-B2206007 (pre-release)

What's changed since v0.3.0:

- Engineering:
  - Bump Newtonsoft.Json to v13.0.1.
    [#73](https://github.com/microsoft/PSRule.Monitor/pull/73)

## v0.3.0

What's changed since v0.2.0:

- New features:
  - Added support for passing through rule annotations.
    [#29](https://github.com/microsoft/PSRule.Monitor/issues/29)
  - Added convention to support ingestion in a pipeline.
    [#46](https://github.com/microsoft/PSRule.Monitor/issues/46)
    - To use this feature, include the `Monitor.LogAnalytics.Import` convention.
- General improvements:
  - Ignore null or empty field and data properties.
    [#44](https://github.com/microsoft/PSRule.Monitor/issues/44)
  - Added `CorrelationId` to ingested events.
    [#45](https://github.com/microsoft/PSRule.Monitor/issues/44)
  - Added configuration of workspace parameter through environment variable.
    [#32](https://github.com/microsoft/PSRule.Monitor/issues/32)
    - Set `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_ID` to configure the workspace id.
    - Set `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_KEY` to configure the shared key.
  - Added `Duration_d` that duration in milliseconds that the rule took to execute.
    [#49](https://github.com/microsoft/PSRule.Monitor/issues/49)

What's changed since pre-release v0.3.0-B2108014:

- No additional changes.

## v0.3.0-B2108014 (pre-release)

What's changed since pre-release v0.3.0-B2108008:

- General improvements:
  - Added `Duration_d` that duration in milliseconds that the rule took to execute.
    [#49](https://github.com/microsoft/PSRule.Monitor/issues/49)

## v0.3.0-B2108008 (pre-release)

What's changed since v0.2.0:

- New features:
  - Added support for passing through rule annotations.
    [#29](https://github.com/microsoft/PSRule.Monitor/issues/29)
  - Added convention to support ingestion in a pipeline.
    [#46](https://github.com/microsoft/PSRule.Monitor/issues/46)
    - To use this feature, include the `Monitor.LogAnalytics.Import` convention.
- General improvements:
  - Ignore null or empty field and data properties.
    [#44](https://github.com/microsoft/PSRule.Monitor/issues/44)
  - Added `CorrelationId` to ingested events.
    [#45](https://github.com/microsoft/PSRule.Monitor/issues/44)
  - Added configuration of workspace parameter through environment variable.
    [#32](https://github.com/microsoft/PSRule.Monitor/issues/32)
    - Set `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_ID` to configure the workspace id.
    - Set `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_KEY` to configure the shared key.

## v0.2.0

What's changed since v0.1.0:

- New features:
  - Added support for passing through data and field properties.
    [#31](https://github.com/microsoft/PSRule.Monitor/issues/31)
  - Added support for passing through rule module name.
    [#30](https://github.com/microsoft/PSRule.Monitor/issues/30)

What's changed since pre-release v0.2.0-B2104009:

- No additional changes.

## v0.2.0-B2104009 (pre-release)

What's changed since v0.1.0:

- New features:
  - Added support for passing through data and field properties.
    [#31](https://github.com/microsoft/PSRule.Monitor/issues/31)
  - Added support for passing through rule module name.
    [#30](https://github.com/microsoft/PSRule.Monitor/issues/30)

## v0.1.0

What's changed since pre-release v0.1.0-B1912005:

- Bug fixes:
  - Removed module dependency on `PSRule`.
    [#13](https://github.com/microsoft/PSRule.Monitor/issues/13)

## v0.1.0-B1912005 (pre-release)

- Initial pre-release.
