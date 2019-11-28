// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using PSRule.Monitor.Data;
using System;
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
    }

    internal sealed class InjestPipelineBuilder : PipelineBuilderBase, IInjestPipelineBuilder
    {
        private string _WorkspaceId;
        private SecureString _SharedKey;
        private string _LogName = "PSRuleTest";

        public void WorkspaceId(string workspaceId)
        {
            _WorkspaceId = workspaceId;
        }

        public void SharedKey(SecureString sharedKey)
        {
            _SharedKey = sharedKey;
        }

        public override IPipeline Build()
        {
            return new InjestPipeline(PrepareContext(), PrepareReader(), _WorkspaceId, _SharedKey, _LogName);
        }
    }

    internal sealed class InjestPipeline : PipelineBase
    {
        private const string ContentType = "application/json";

        private const string HEADER_ACCEPT = "Accept";
        private const string HEADER_AUTHORIZATION = "Authorization";
        private const string HEADER_LOGTYPE = "Log-Type";
        private const string HEADER_DATE = "x-ms-date";
        private const string HEADER_RESOURCEID = "x-ms-AzureResourceId";
        private const string HEADER_TIMEGENERATED = "time-generated-field";

        private readonly string _LogName;
        private readonly Uri _EndpointUri;
        private readonly CollectionHash _Hash;
        private readonly BatchQueue _SubmissionQueue;

        static string TimeStampField = "";

        internal InjestPipeline(PipelineContext context, PipelineReader reader, string workspaceId, SecureString sharedKey, string logName)
            : base(context, reader)
        {
            _LogName = logName;
            _EndpointUri = new Uri(string.Concat("https://", workspaceId, ".ods.opinsights.azure.com/api/logs?api-version=2016-04-01"));
            _Hash = new CollectionHash(workspaceId, sharedKey);
            _SubmissionQueue = new BatchQueue();
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

            var ruleName = GetProperty<string>(sourceObject, "ruleName");
            var targetName = GetProperty<string>(sourceObject, "targetName");
            var targetType = GetProperty<string>(sourceObject, "targetType");
            var outcome = GetProperty<string>(sourceObject, "outcome");

            var record = new LogRecord
            {
                RuleName = ruleName,
                TargetName = targetName,
                TargetType = targetType,
                Outcome = outcome
            };
            return record;
        }

        private static T GetProperty<T>(PSObject obj, string propertyName)
        {
            return null == obj.Properties[propertyName] ? default(T) : (T)obj.Properties[propertyName].Value;
        }

        /// <summary>
        /// Submits a batch of records to Azure Monitor data collector.
        /// </summary>
        private void SubmitBatch(LogRecord[] records)
        {
            var json = JsonConvert.SerializeObject(records);

            // Create a hash for the API signature
            var date = DateTime.UtcNow;
            var data = Encoding.UTF8.GetBytes(json);
            var signature = _Hash.ComputeSignature(data.Length, date, ContentType);
            PostData(PrepareRequest(signature, date, json));
        }

        /// <summary>
        /// Post log data to Azure Monitor endpoint.
        /// </summary>
        private void PostData(HttpRequestMessage request)
        {
            var response = GetClient().SendAsync(request);
            response.Wait();
            var result = response.Result.Content.ReadAsStringAsync().Result;
        }

        private HttpClient GetClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(HEADER_ACCEPT, ContentType);
            client.DefaultRequestHeaders.Add(HEADER_LOGTYPE, _LogName);
            return client;
        }

        private HttpRequestMessage PrepareRequest(string signature, DateTime date, string json)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _EndpointUri);
            request.Headers.Add(HEADER_AUTHORIZATION, signature);
            request.Headers.Add(HEADER_DATE, date.ToString("r"));
            request.Headers.Add(HEADER_TIMEGENERATED, TimeStampField);
            request.Content = new StringContent(json, Encoding.UTF8, ContentType);
            return request;
        }
    }
}
