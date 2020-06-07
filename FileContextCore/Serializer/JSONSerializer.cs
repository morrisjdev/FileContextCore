using System;
using System.Collections.Generic;
using System.Linq;
using FileContextCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;

namespace FileContextCore.Serializer
{
    public class JSONSerializer : ISerializer
    {
        private IEntityType _entityType;
        private object _keyValueFactory;
        private string[] _propertyKeys;
        private Type[] _typeList;

        public void Initialize(IFileContextScopedOptions _, IEntityType entityType, object keyValueFactory)
        {
            _keyValueFactory = keyValueFactory;
            _entityType = entityType;
            _propertyKeys = _entityType.GetProperties().Select(p => p.GetColumnName()).ToArray();
            _typeList = _entityType.GetProperties().Select(p => p.GetValueConverter()?.ProviderClrType ?? p.ClrType).ToArray();
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList)
        {
            if(list != String.Empty)
            {
                JArray array = JArray.Parse(list);

                foreach (var jToken in array)
                {
                    var json = (JObject) jToken;
                    List<object> value = new List<object>();

                    for (int i = 0; i < _propertyKeys.Length; i++)
                    {
                        object val = json.Value<string>(_propertyKeys[i]).Deserialize(_typeList[i]);
                        value.Add(val);
                    }

                    TKey key = SerializerHelper.GetKey<TKey>(_keyValueFactory, _entityType,
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

                for (int i = 0; i < _propertyKeys.Length; i++)
                {
                    JProperty property = new JProperty(_propertyKeys[i], val.Value[i].Serialize());
                    json.Add(property);
                }

                array.Add(json);
            }

            return array.ToString();
        }

        public string FileType => "json";
    }
}
