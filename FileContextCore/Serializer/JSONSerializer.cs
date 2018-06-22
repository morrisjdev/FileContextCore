using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileContextCore.Serializer
{
    class JSONSerializer : ISerializer
    {
        private IEntityType entityType;
        private string[] propertyKeys;
        private readonly Type[] typeList;

        public JSONSerializer(IEntityType _entityType)
        {
            entityType = _entityType;
            propertyKeys = entityType.GetProperties().Select(p => p.Name).ToArray();
            typeList = entityType.GetProperties().Select(p => p.ClrType).ToArray();
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList)
        {
            if(list != String.Empty)
            {
                JArray array = JArray.Parse(list);

                foreach (JObject json in array)
                {
                    TKey key = (TKey)json.Value<string>("Key").Deserialize(typeof(TKey));
                    List<object> value = new List<object>();

                    for (int i = 0; i < propertyKeys.Length; i++)
                    {
                        object val = json.Value<string>(propertyKeys[i]).Deserialize(typeList[i]);
                        value.Add(val);
                    }

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
                JObject json = new JObject
                {
                    new JProperty("Key", val.Key.Serialize())
                };

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
