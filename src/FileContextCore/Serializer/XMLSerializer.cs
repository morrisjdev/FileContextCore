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

        public List<T> DeserializeList<T>(string list)
        {
            if(list != "")
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<T>));
                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(list));

                List<T> result = (List<T>)xs.Deserialize(memoryStream);
                memoryStream.Dispose();

                return result;
            }

            return new List<T>();
        }

        public string SerializeList<T>(List<T> list)
        {
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();

            Type t = typeof(T);
            PropertyInfo[] virtualProperties = t.GetPropertiesNotForSerialize();

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

        public T Deserialize<T>(string obj)
        {
            if(obj != "")
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(obj));

                T result = (T)xs.Deserialize(memoryStream);
                memoryStream.Dispose();

                return result;
            }

            return default(T);
        }

        public string Serialize<T>(T obj)
        {
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();

            Type t = obj.GetType();
            PropertyInfo[] virtualProperties = t.GetPropertiesNotForSerialize();

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
