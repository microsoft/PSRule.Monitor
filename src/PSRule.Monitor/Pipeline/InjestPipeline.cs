// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using PSRule.Monitor.Data;
using System;
using System.Collections;
using System.Globalization;
using System.Management.Automation;
using System.Net.Http;
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
            return new InjestPipeline(PrepareContext(), PrepareReader(), _WorkspaceId, _SharedKey, _LogName);
        }
    }

    internal sealed class InjestPipeline : PipelineBase
    {
        private const string CONTENTTYPE = "application/json";
        private const string TIMESTAMPFIELD = "";
        private const string APIVERSION = "2016-04-01";

        private const string HEADER_ACCEPT = "Accept";
        private const string HEADER_AUTHORIZATION = "Authorization";
        private const string HEADER_LOGTYPE = "Log-Type";
        private const string HEADER_DATE = "x-ms-date";
        private const string HEADER_RESOURCEID = "x-ms-AzureResourceId";
        private const string HEADER_TIMEGENERATED = "time-generated-field";

        private static readonly CultureInfo FormatCulture = new CultureInfo("en-US");

        private readonly string _LogName;
        private readonly Uri _EndpointUri;
        private readonly CollectionHash _Hash;
        private readonly BatchQueue _SubmissionQueue;
        private readonly HttpClient _HttpClient;

        internal InjestPipeline(PipelineContext context, PipelineReader reader, string workspaceId, SecureString sharedKey, string logName)
            : base(context, reader)
        {
            _LogName = logName;
            _EndpointUri = new Uri(string.Concat("https://", workspaceId, ".ods.opinsights.azure.com/api/logs?api-version=", APIVERSION));
            _Hash = new CollectionHash(workspaceId, sharedKey);
            _SubmissionQueue = new BatchQueue();
            _HttpClient = GetClient();
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
            var field = GetProperty<object>(sourceObject, "field");
            var resourceId = GetField(field, "resourceId");
            var record = new LogRecord
            {
                RuleName = ruleName,
                TargetName = targetName,
                TargetType = targetType,
                Outcome = outcome,
                ResourceId = resourceId
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

        private static string GetField(object obj, string propertyName)
        {
            if (obj is IDictionary dictionary && TryDictionary(dictionary, propertyName, out object value) && value != null)
                return value.ToString();

            if (obj is PSObject pso)
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
            using (var request = PrepareRequest(signature, date, resourceId, json))
            {
                var response = _HttpClient.SendAsync(request);
                response.Wait();
                var result = response.Result.Content.ReadAsStringAsync().Result;
            }
        }

        private HttpClient GetClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(HEADER_ACCEPT, CONTENTTYPE);
            client.DefaultRequestHeaders.Add(HEADER_LOGTYPE, _LogName);
            return client;
        }

        private HttpRequestMessage PrepareRequest(string signature, DateTime date, string resourceId, string json)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _EndpointUri);
            request.Headers.Add(HEADER_AUTHORIZATION, signature);
            request.Headers.Add(HEADER_DATE, date.ToString("r", FormatCulture));
            request.Headers.Add(HEADER_TIMEGENERATED, TIMESTAMPFIELD);
            request.Headers.Add(HEADER_RESOURCEID, resourceId);
            request.Content = new StringContent(json, Encoding.UTF8, CONTENTTYPE);
            return request;
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Hash.Dispose();
                _HttpClient.Dispose();
            }
        }

        #endregion IDisposable
    }
}
