---
external help file: PSRule.Monitor-help.xml
Module Name: PSRule.Monitor
online version: https://github.com/Microsoft/PSRule.Monitor/blob/master/docs/commands/PSRule.Monitor/en-US/Send-PSRuleMonitorRecord.md
schema: 2.0.0
---

# Send-PSRuleMonitorRecord

## SYNOPSIS

Send analysis results from PSRule to Azure Monitor.

## SYNTAX

```text
Send-PSRuleMonitorRecord [-WorkspaceId] <String> [-SharedKey] <SecureString> [[-InputObject] <PSObject>]
 [<CommonParameters>]
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

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SharedKey

Either the primary or secondary key for the workspace.

```yaml
Type: SecureString
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
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
