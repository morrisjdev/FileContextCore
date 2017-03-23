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

        public IList DeserializeList(string list, Type t)
        {
            return JsonConvert.DeserializeObject(list, typeof(List<>).MakeGenericType(t), settings) as IList;
        }

        public string SerializeList(IList list)
        {
            return JsonConvert.SerializeObject(list, settings);
        }

        public object Deserialize(string obj, Type t)
        {
            return JsonConvert.DeserializeObject(obj, t, settings);
        }

        public string Serialize(object obj)
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

            if(propInfo != null)
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
