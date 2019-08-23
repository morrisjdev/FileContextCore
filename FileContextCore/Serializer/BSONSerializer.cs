using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace FileContextCore.Serializer
{
    class BSONSerializer<T> : ISerializer
    {
        private IEntityType entityType;
        private readonly IPrincipalKeyValueFactory<T> _keyValueFactory;
        private string[] propertyKeys;
        private readonly Type[] typeList;

        public BSONSerializer(IEntityType _entityType, IPrincipalKeyValueFactory<T> _keyValueFactory)
        {
            entityType = _entityType;
            this._keyValueFactory = _keyValueFactory;
            propertyKeys = entityType.GetProperties().Select(p => p.Relational().ColumnName).ToArray();
            typeList = entityType.GetProperties().Select(p => p.GetValueConverter()?.ProviderClrType ?? p.ClrType).ToArray();
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList)
        {
            byte[] data = Convert.FromBase64String(list);

            MemoryStream ms = new MemoryStream(data);

            using(BsonDataReader reader = new BsonDataReader(ms))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
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

                TKey key = SerializerHelper.GetKey<TKey, T>(_keyValueFactory, entityType,
                    propertyName => json.Value<string>(propertyName));

                List<object> value = new List<object>();

                for (int i = 0; i < propertyKeys.Length; i++)
                {
                    object val = json.Value<string>(propertyKeys[i]).Deserialize(typeList[i]);
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

                    for (int i = 0; i < propertyKeys.Length; i++)
                    {
                        writer.WritePropertyName(propertyKeys[i]);
                        writer.WriteValue(val.Value[i].Serialize());
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
