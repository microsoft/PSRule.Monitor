# Change log

## Unreleased

## v0.3.0

What's changed since v0.2.0:

- New features:
  - Added support for passing through rule annotations. [#29](https://github.com/microsoft/PSRule.Monitor/issues/29)
  - Added convention to support ingestion in a pipeline. [#46](https://github.com/microsoft/PSRule.Monitor/issues/46)
    - To use this feature, include the `Monitor.LogAnalytics.Import` convention.
- General improvements:
  - Ignore null or empty field and data properties. [#44](https://github.com/microsoft/PSRule.Monitor/issues/44)
  - Added `CorrelationId` to ingested events. [#45](https://github.com/microsoft/PSRule.Monitor/issues/44)
  - Added configuration of workspace parameter through environment variable. [#32](https://github.com/microsoft/PSRule.Monitor/issues/32)
    - Set `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_ID` to configure the workspace id.
    - Set `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_KEY` to configure the shared key.
  - Added `Duration_d` that duration in milliseconds that the rule took to execute. [#49](https://github.com/microsoft/PSRule.Monitor/issues/49)

What's changed since pre-release v0.3.0-B2108014:

- No additional changes.

## v0.3.0-B2108014 (pre-release)

What's changed since pre-release v0.3.0-B2108008:

- General improvements:
  - Added `Duration_d` that duration in milliseconds that the rule took to execute. [#49](https://github.com/microsoft/PSRule.Monitor/issues/49)

## v0.3.0-B2108008 (pre-release)

What's changed since v0.2.0:

- New features:
  - Added support for passing through rule annotations. [#29](https://github.com/microsoft/PSRule.Monitor/issues/29)
  - Added convention to support ingestion in a pipeline. [#46](https://github.com/microsoft/PSRule.Monitor/issues/46)
    - To use this feature, include the `Monitor.LogAnalytics.Import` convention.
- General improvements:
  - Ignore null or empty field and data properties. [#44](https://github.com/microsoft/PSRule.Monitor/issues/44)
  - Added `CorrelationId` to ingested events. [#45](https://github.com/microsoft/PSRule.Monitor/issues/44)
  - Added configuration of workspace parameter through environment variable. [#32](https://github.com/microsoft/PSRule.Monitor/issues/32)
    - Set `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_ID` to configure the workspace id.
    - Set `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_KEY` to configure the shared key.

## v0.2.0

What's changed since v0.1.0:

- New features:
  - Added support for passing through data and field properties. [#31](https://github.com/microsoft/PSRule.Monitor/issues/31)
  - Added support for passing through rule module name. [#30](https://github.com/microsoft/PSRule.Monitor/issues/30)

What's changed since pre-release v0.2.0-B2104009:

- No additional changes.

## v0.2.0-B2104009 (pre-release)

What's changed since v0.1.0:

- New features:
  - Added support for passing through data and field properties. [#31](https://github.com/microsoft/PSRule.Monitor/issues/31)
  - Added support for passing through rule module name. [#30](https://github.com/microsoft/PSRule.Monitor/issues/30)

## v0.1.0

What's changed since pre-release v0.1.0-B1912005:

- Bug fixes:
  - Removed module dependency on `PSRule`. [#13](https://github.com/microsoft/PSRule.Monitor/issues/13)

## v0.1.0-B1912005 (pre-release)

- Initial pre-release.
