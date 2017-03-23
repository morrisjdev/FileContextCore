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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

namespace FileContextCore.Serializer
{
    public class XMLSerializer : ISerializer
    {
        private XmlWriterSettings settings = new XmlWriterSettings()
        {
            OmitXmlDeclaration = true
        };

        public XMLSerializer(bool indent = true)
        {
            settings.Indent = indent;
        }

        public string FileType { get { return "xml"; } }

        public IList DeserializeList(string list, Type t)
        {
            if(list != "")
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<>).MakeGenericType(t));
                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(list));

                return (IList)xs.Deserialize(memoryStream);
            }

            return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
        }

        public string SerializeList(IList list)
        {
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();

            Type t = list.GetType().GenericTypeArguments[0];
            PropertyInfo[] virtualProperties = t.GetRuntimeProperties().Where(x => x.SetMethod.IsVirtual).ToArray();

            XmlAttributes attributes = new XmlAttributes()
            {
                XmlIgnore = true
            };

            foreach(PropertyInfo pi in virtualProperties)
            {
                overrides.Add(t, pi.Name, attributes);
            }

            XmlSerializer xs = new XmlSerializer(list.GetType(), overrides);
            StringWriter sw = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sw, settings);
            xs.Serialize(writer, list);
            return sw.ToString();
        }

        public object Deserialize(string obj, Type t)
        {
            if(obj != "")
            {
                XmlSerializer xs = new XmlSerializer(t);
                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(obj));

                return xs.Deserialize(memoryStream);
            }

            return null;
        }

        public string Serialize(object obj)
        {
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();

            Type t = obj.GetType();
            PropertyInfo[] virtualProperties = t.GetRuntimeProperties().Where(x => x.SetMethod.IsVirtual).ToArray();

            XmlAttributes attributes = new XmlAttributes()
            {
                XmlIgnore = true
            };

            foreach (PropertyInfo pi in virtualProperties)
            {
                overrides.Add(t, pi.Name, attributes);
            }

            XmlSerializer xs = new XmlSerializer(t, overrides);
            StringWriter sw = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sw, settings);
            xs.Serialize(writer, obj);
            return sw.ToString();
        }
    }
}
