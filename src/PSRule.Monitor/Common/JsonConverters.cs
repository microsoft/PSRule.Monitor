// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;
using System.Collections;

namespace PSRule.Monitor
{
    /// <summary>
    /// A JSON converter to convert an object into a flat string.
    /// </summary>
    internal sealed class StringifyMapConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Hashtable).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is Hashtable map))
                return;

            var v = JsonConvert.SerializeObject(map);
            writer.WriteValue(v);
        }
    }
}
