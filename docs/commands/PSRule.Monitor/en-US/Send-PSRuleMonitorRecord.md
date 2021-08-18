---
external help file: PSRule.Monitor-help.xml
Module Name: PSRule.Monitor
online version: https://github.com/Microsoft/PSRule.Monitor/blob/main/docs/commands/PSRule.Monitor/en-US/Send-PSRuleMonitorRecord.md
schema: 2.0.0
---

# Send-PSRuleMonitorRecord

## SYNOPSIS

Send analysis results from PSRule to Azure Monitor.

## SYNTAX

```text
Send-PSRuleMonitorRecord [[-WorkspaceId] <String>] [[-SharedKey] <SecureString>] [[-InputObject] <PSObject>]
 [-LogName <String>] [<CommonParameters>]
```

## DESCRIPTION

Send analysis results from PSRule as logs into an Azure Monitor Log Analytics workspace.

Generate analysis results from PSRule by using the Invoke-PSRule cmdlet.
Results are automatically batched and inserted as custom log data within the PSRule_CL table.

## EXAMPLES

### Example 1

```powershell
$data | Invoke-PSRule | Send-PSRuleMonitorRecord;
```

Send results from the pipeline to Azure Monitor.

### Example 2

```powershell
Get-Content .\output.json -Raw | ConvertFrom-Json | Send-PSRuleMonitorRecord;
```

Send results from disk to Azure Monitor.

### Example 3

```powershell
$results = Invoke-PSRule -InputPath .\*.json -Module 'PSRule.Rules.Azure';
$results | Send-PSRuleMonitorRecord -WorkspaceId <workspaceId> -SharedKey <primaryKey>;
```

Send results from a variable to Azure Monitor.

## PARAMETERS

### -WorkspaceId

The unique identifier of a Azure Monitor Log Analytics workspace.
If not specified, the `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_ID` environment variable is used.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SharedKey

Either the primary or secondary key for the workspace.
If not specified, the `PSRULE_CONFIGURATION_MONITOR_WORKSPACE_KEY` environment variable is used.

```yaml
Type: SecureString
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LogName

Optionally specifies an alternative data source to store log data into.
By default PSRule is used.

If the log does not already exist, it is created.
It can create up to 15 minutes to create the log initially.

The specified log name will have the _CL suffix appended to it.
For example, when the log name is PSRule the use PSRule_CL in log queries.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: PSRule
Accept pipeline input: False
Accept wildcard characters: False
```

### -InputObject

The analysis record to send to Azure Monitor.

```yaml
Type: PSObject
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Management.Automation.PSObject

## OUTPUTS

### System.Void

## NOTES

## RELATED LINKS
