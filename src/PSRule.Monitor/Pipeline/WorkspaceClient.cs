// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Management.Automation;
using System.Security;
using System.Text;
using Newtonsoft.Json;
using PSRule.Monitor.Data;

namespace PSRule.Monitor.Pipeline;

internal sealed class WorkspaceClient(string workspaceId, SecureString sharedKey, ILogClient logClient) : MonitorClient
{
    private const string CONTENT_TYPE = "application/json";

    private readonly CollectionHash _Hash = new(workspaceId, sharedKey);
    private readonly BatchQueue _SubmissionQueue = new();
    private readonly ILogClient _LogClient = logClient;
    private readonly Guid _CorrelationId = Guid.NewGuid();

    // Track whether Dispose has been called.
    private bool _Disposed;

    public void Enqueue(PSObject result)
    {
        _SubmissionQueue.Enqueue(ProcessResult(result));
    }

    public void Send(int minSize, int maxSize)
    {
        while (_SubmissionQueue.TryDequeue(minSize, maxSize, out var records))
            SubmitBatch(records);
    }

    public void Send()
    {
        Send(0, 100);
    }

    /// <summary>
    /// Submits a batch of records to Azure Monitor data collector.
    /// </summary>
    private void SubmitBatch(LogRecord[] records)
    {
        var json = JsonConvert.SerializeObject(records);
        var resourceId = records[0].ResourceId;

        // Create a hash for the API signature
        var date = DateTime.UtcNow;
        var data = Encoding.UTF8.GetBytes(json);
        var signature = _Hash.ComputeSignature(data.Length, date, CONTENT_TYPE);
        PostData(signature, date, resourceId, json);
    }

    /// <summary>
    /// Maps a RuleRecord to a LogRecord.
    /// </summary>
    private LogRecord ProcessResult(PSObject sourceObject)
    {
        if (sourceObject == null)
            return null;

        var ruleId = GetPropertyValue(sourceObject, "ruleId");
        var ruleName = GetPropertyValue(sourceObject, "ruleName");
        var targetName = GetPropertyValue(sourceObject, "targetName");
        var targetType = GetPropertyValue(sourceObject, "targetType");
        var outcome = GetPropertyValue(sourceObject, "outcome");
        var data = GetProperty<object>(sourceObject, "data");
        var field = GetProperty<object>(sourceObject, "field");
        var info = GetProperty<object>(sourceObject, "info");
        var resourceId = GetField(data, "resourceId") ?? GetField(field, "resourceId");
        var displayName = GetField(info, "displayName") ?? ruleName;
        var moduleName = GetField(info, "moduleName");
        var annotations = GetProperty(info, "annotations");
        var runId = GetPropertyValue(sourceObject, "runId");
        var duration = GetProperty<long>(sourceObject, "time");
        var record = new LogRecord
        {
            RuleId = ruleId,
            RuleName = ruleName,
            DisplayName = displayName,
            ModuleName = moduleName,
            TargetName = targetName,
            TargetType = targetType,
            Outcome = outcome,
            ResourceId = resourceId,
            Data = GetPropertyMap(data),
            Field = GetPropertyMap(field),
            Annotations = GetPropertyMap(annotations),
            RunId = runId,
            CorrelationId = _CorrelationId,
            Duration = duration,
        };
        return record;
    }

    /// <summary>
    /// Post log data to Azure Monitor endpoint.
    /// </summary>
    private void PostData(string signature, DateTime date, string resourceId, string json)
    {
        _LogClient.Post(signature, date, resourceId, json);
    }

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
            {
                _Hash.Dispose();
                _LogClient.Dispose();
            }
            _Disposed = true;
        }
        base.Dispose(disposing);
    }

    #endregion IDisposable
}
