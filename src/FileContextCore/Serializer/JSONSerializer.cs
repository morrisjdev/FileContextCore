using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FileContextCore.Helper;
using Newtonsoft.Json.Linq;
using System.Collections;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace FileContextCore.Serializer
{
    public class JSONSerializer : ISerializer
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            ContractResolver = new PreserveVirtualResolver()
        };

        public JSONSerializer(Formatting formatting = Formatting.Indented)
        {
            settings.Formatting = formatting;
        }

        public string FileType { get { return "json"; } }

        public List<T> DeserializeList<T>(string list)
        {
            return JsonConvert.DeserializeObject<List<T>>(list, settings);
        }

        public string SerializeList<T>(List<T> list)
        {
            return JsonConvert.SerializeObject(list, settings);
        }

        public T Deserialize<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj, settings);
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }
    }

    class PreserveVirtualResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);
            PropertyInfo propInfo = (PropertyInfo)member;

            prop.Ignored = false;

            if (propInfo != null)
            {
                if(propInfo.GetMethod.IsVirtual && !propInfo.GetMethod.IsFinal)
                {
                    prop.ShouldSerialize = obj => false;
                }
            }

            return prop;
        }
    }
}
