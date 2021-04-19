// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSRule.Monitor.Configuration;
using PSRule.Monitor.Pipeline;
using System;
using System.Management.Automation;
using System.Security;
using System.Text;
using Xunit;

namespace PSRule.Monitor
{
    public sealed class InjestPipelineTests
    {
        [Fact]
        public void InvokePipeline()
        {
            var pipeline = GetPipeline(out TestLogClient logClient);
            pipeline.Begin();
            var o = GetObject();
            pipeline.Process(o);
            pipeline.End();

            Assert.Single(logClient.Output);
            var actual = logClient.Output[0];
            var actualJson = JsonConvert.DeserializeObject<JArray>(actual.Json);

            Assert.Equal("test-resource-id", actual.ResourceId);
            Assert.NotEmpty(actual.Signature);

            Assert.Equal(o.Properties["ruleName"].Value, actualJson[0]["RuleName"].Value<string>());
            Assert.Equal(o.Properties["targetName"].Value, actualJson[0]["TargetName"].Value<string>());
            Assert.Equal(o.Properties["targetType"].Value, actualJson[0]["TargetType"].Value<string>());
            Assert.Equal(o.Properties["outcome"].Value, actualJson[0]["Outcome"].Value<string>());
            Assert.Equal(o.Properties["ruleName"].Value, actualJson[0]["DisplayName"].Value<string>());
            Assert.Equal("test-module", actualJson[0]["ModuleName"].Value<string>());
        }

        #region Helper methods

        private const string _WorkspaceId = "00000000-0000-0000-0000-000000000000";

        private static PSObject GetObject()
        {
            var info = new PSObject();
            info.Properties.Add(new PSNoteProperty("moduleName", "test-module"));

            var data = new PSObject();
            data.Properties.Add(new PSNoteProperty("resourceId", "test-resource-id"));

            var o = new PSObject();
            o.Properties.Add(new PSNoteProperty("ruleName", "test-rule"));
            o.Properties.Add(new PSNoteProperty("targetName", "test-name"));
            o.Properties.Add(new PSNoteProperty("targetType", "test-type"));
            o.Properties.Add(new PSNoteProperty("outcome", "Fail"));
            o.Properties.Add(new PSNoteProperty("info", info));
            o.Properties.Add(new PSNoteProperty("data", data));
            return o;
        }

        private static IPipeline GetPipeline(out TestLogClient logClient)
        {
            var key = new SecureString();
            foreach (var c in Convert.ToBase64String(Encoding.UTF8.GetBytes(_WorkspaceId)))
                key.AppendChar(c);

            logClient = new TestLogClient();
            return new InjestPipeline(GetContent(), GetReader(), _WorkspaceId, key, logClient);
        }

        private static PipelineReader GetReader()
        {
            return new PipelineReader();
        }

        private static PipelineContext GetContent()
        {
            return new PipelineContext(new PSRuleOption());
        }

        #endregion Helper methods
    }
}
