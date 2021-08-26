# Sample queries

## Slowest rules

Get the top 10 slowest rules to execute by average execution time in milliseconds.

```kusto
PSRule_CL
| where TimeGenerated >= ago(30d) and isnotnull(Duration_d)
| summarize Avg_ms = avg(Duration_d), Max_ms = max(Duration_d), Min_ms = min(Duration_d), Count = count() by RuleId_s
| top 10 by Avg_ms desc nulls last 
```
