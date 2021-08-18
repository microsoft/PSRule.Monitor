// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using PSRule.Monitor.Data;
using System;
using System.Management.Automation;
using System.Security;
using System.Text;

namespace PSRule.Monitor.Pipeline
{
    internal static class WorkspaceClientExtensions
    {
        public static void Enqueue(this WorkspaceClient client, PSObject[] results)
        {
            if (results == null || results.Length == 0)
                return;

            for (var i = 0; i < results.Length; i++)
                client.Enqueue(results[i]);
        }
    }

    internal sealed class WorkspaceClient : MonitorClient
    {
        private const string CONTENTTYPE = "application/json";

        private readonly CollectionHash _Hash;
        private readonly BatchQueue _SubmissionQueue;
        private readonly ILogClient _LogClient;
        private readonly Guid _CorrelationId;

        // Track whether Dispose has been called.
        private bool _Disposed;

        public WorkspaceClient(string workspaceId, SecureString sharedKey, ILogClient logClient)
        {
            _Hash = new CollectionHash(workspaceId, sharedKey);
            _SubmissionQueue = new BatchQueue();
            _LogClient = logClient;
            _CorrelationId = Guid.NewGuid();
        }

        public void Enqueue(PSObject result)
        {
            _SubmissionQueue.Enqueue(ProcessResult(result));
        }

        public void Send(int minSize, int maxSize)
        {
            while (_SubmissionQueue.TryDequeue(minSize, maxSize, out LogRecord[] records))
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
            var signature = _Hash.ComputeSignature(data.Length, date, CONTENTTYPE);
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
}
