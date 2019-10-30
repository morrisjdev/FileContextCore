using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace FileContextCore.Serializer
{
    class JSONSerializer<T> : ISerializer
    {
        private IEntityType entityType;
        private readonly IPrincipalKeyValueFactory<T> _keyValueFactory;
        private string[] propertyKeys;
        private readonly Type[] typeList;

        public JSONSerializer(IEntityType _entityType, IPrincipalKeyValueFactory<T> _keyValueFactory)
        {
            entityType = _entityType;
            this._keyValueFactory = _keyValueFactory;
            propertyKeys = entityType.GetProperties().Select(p => p.GetColumnName()).ToArray();
            typeList = entityType.GetProperties().Select(p => p.GetValueConverter()?.ProviderClrType ?? p.ClrType).ToArray();
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList)
        {
            if(list != String.Empty)
            {
                JArray array = JArray.Parse(list);

                foreach (JObject json in array)
                {
                    List<object> value = new List<object>();

                    for (int i = 0; i < propertyKeys.Length; i++)
                    {
                        object val = json.Value<string>(propertyKeys[i]).Deserialize(typeList[i]);
                        value.Add(val);
                    }

                    TKey key = SerializerHelper.GetKey<TKey, T>(_keyValueFactory, entityType,
                        propertyName => json.Value<string>(propertyName));

                    newList.Add(key, value.ToArray());
                }
            }

            return newList;
        }

        public string Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            JArray array = new JArray();

            foreach(KeyValuePair<TKey, object[]> val in list)
            {
                JObject json = new JObject();

                for (int i = 0; i < propertyKeys.Length; i++)
                {
                    JProperty property = new JProperty(propertyKeys[i], val.Value[i].Serialize());
                    json.Add(property);
                }

                array.Add(json);
            }

            return array.ToString();
        }
    }
}
