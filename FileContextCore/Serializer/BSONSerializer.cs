using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileContextCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace FileContextCore.Serializer
{
    public class BSONSerializer : ISerializer
    {
        private IEntityType _entityType;
        private object _keyValueFactory;
        private string[] _propertyKeys;
        private Type[] _typeList;

        public void Initialize(IFileContextScopedOptions options, IEntityType entityType, object keyValueFactory)
        {
            _entityType = entityType;
            _keyValueFactory = keyValueFactory;
            _propertyKeys = _entityType.GetProperties().Select(p => p.GetColumnName()).ToArray();
            _typeList = _entityType.GetProperties().Select(p => p.GetValueConverter()?.ProviderClrType ?? p.ClrType).ToArray();
        }
        
        public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList)
        {
            byte[] data = Convert.FromBase64String(list);

            MemoryStream ms = new MemoryStream(data);

            using(BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                JObject array = serializer.Deserialize<JObject>(reader);

                if (array != null)
                {
                    DeserializeValues(array, newList);

                }
            }

            return newList;
        }

        private void DeserializeValues<TKey>(JObject array, Dictionary<TKey, object[]> newList)
        {
            JProperty current = (JProperty)array.First;

            while (current != null)
            {
                JObject json = (JObject)current.Value;

                TKey key = SerializerHelper.GetKey<TKey>(_keyValueFactory, _entityType,
                    propertyName => json.Value<string>(propertyName));

                List<object> value = new List<object>();

                for (int i = 0; i < _propertyKeys.Length; i++)
                {
                    object val = json.Value<string>(_propertyKeys[i]).Deserialize(_typeList[i]);
                    value.Add(val);
                }

                newList.Add(key, value.ToArray());

                current = (JProperty)current.Next;
            }
        }

        public string Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            MemoryStream ms = new MemoryStream();

            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                writer.WriteStartArray();

                foreach (KeyValuePair<TKey, object[]> val in list)
                {
                    writer.WriteStartObject();

                    for (int i = 0; i < _propertyKeys.Length; i++)
                    {
                        writer.WritePropertyName(_propertyKeys[i]);
                        writer.WriteValue(val.Value[i].Serialize());
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string FileType => "bson";
    }
}
