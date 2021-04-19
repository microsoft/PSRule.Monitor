// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using PSRule.Monitor.Data;
using System;
using System.Collections;
using System.Management.Automation;
using System.Security;
using System.Text;

namespace PSRule.Monitor.Pipeline
{
    public interface IInjestPipelineBuilder : IPipelineBuilder
    {
        void WorkspaceId(string workspaceId);

        void SharedKey(SecureString sharedKey);

        void LogName(string logName);
    }

    internal sealed class InjestPipelineBuilder : PipelineBuilderBase, IInjestPipelineBuilder
    {
        private const string DEFAULT_LOGNAME = "PSRule";

        private string _WorkspaceId;
        private SecureString _SharedKey;
        private string _LogName = DEFAULT_LOGNAME;

        public void WorkspaceId(string workspaceId)
        {
            _WorkspaceId = workspaceId;
        }

        public void SharedKey(SecureString sharedKey)
        {
            _SharedKey = sharedKey;
        }

        public void LogName(string logName)
        {
            _LogName = logName;
        }

        public override IPipeline Build()
        {
            var logClient = new LogClient(_WorkspaceId, _LogName);
            return new InjestPipeline(PrepareContext(), PrepareReader(), _WorkspaceId, _SharedKey, logClient);
        }
    }

    internal sealed class InjestPipeline : PipelineBase
    {
        private const string CONTENTTYPE = "application/json";

        private readonly CollectionHash _Hash;
        private readonly BatchQueue _SubmissionQueue;
        private readonly ILogClient _LogClient;

        // Track whether Dispose has been called.
        private bool _Disposed;

        internal InjestPipeline(PipelineContext context, PipelineReader reader, string workspaceId, SecureString sharedKey, ILogClient logClient)
            : base(context, reader)
        {
            _Hash = new CollectionHash(workspaceId, sharedKey);
            _SubmissionQueue = new BatchQueue();
            _LogClient = logClient;
        }

        public override void Process(PSObject sourceObject)
        {
            Reader.Enqueue(sourceObject);
            while (Reader.TryDequeue(out PSObject next))
                _SubmissionQueue.Enqueue(ProcessResult(next));

            while (_SubmissionQueue.TryDequeue(30, 100, out LogRecord[] records))
                SubmitBatch(records);
        }

        public override void End()
        {
            while (_SubmissionQueue.TryDequeue(0, 100, out LogRecord[] records))
                SubmitBatch(records);
        }

        /// <summary>
        /// Maps a RuleRecord to a LogRecord.
        /// </summary>
        private static LogRecord ProcessResult(PSObject sourceObject)
        {
            if (sourceObject == null)
                return null;

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
            var record = new LogRecord
            {
                RuleName = ruleName,
                DisplayName = displayName,
                ModuleName = moduleName,
                TargetName = targetName,
                TargetType = targetType,
                Outcome = outcome,
                ResourceId = resourceId,
                Data = GetPropertyMap(data),
                Field = GetPropertyMap(field),
            };
            return record;
        }

        private static string GetPropertyValue(PSObject obj, string propertyName)
        {
            return obj.Properties[propertyName] == null || obj.Properties[propertyName].Value == null ? null : obj.Properties[propertyName].Value.ToString();
        }

        private static T GetProperty<T>(PSObject obj, string propertyName)
        {
            return obj.Properties[propertyName] == null ? default(T) : (T)obj.Properties[propertyName].Value;
        }

        private static Hashtable GetPropertyMap(object o)
        {
            if (o == null)
                return null;

            var result = new Hashtable();
            if (o is IDictionary dictionary)
            {
                foreach (DictionaryEntry kv in dictionary)
                    result[kv.Key] = kv.Value;
            }
            else if (o is PSObject pso)
            {
                foreach (var p in pso.Properties)
                {
                    if (p.MemberType == PSMemberTypes.NoteProperty)
                        result[p.Name] = p.Value;
                }
            }
            return result;
        }

        private static string GetField(object o, string propertyName)
        {
            if (o is IDictionary dictionary && TryDictionary(dictionary, propertyName, out object value) && value != null)
                return value.ToString();

            if (o is PSObject pso)
                return GetPropertyValue(pso, propertyName);

            return null;
        }

        private static bool TryDictionary(IDictionary dictionary, string key, out object value)
        {
            value = null;
            var comparer = StringComparer.OrdinalIgnoreCase;
            foreach (var k in dictionary.Keys)
            {
                if (comparer.Equals(key, k))
                {
                    value = dictionary[k];
                    return true;
                }
            }
            return false;
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
