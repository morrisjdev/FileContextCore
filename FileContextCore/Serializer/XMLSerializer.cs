using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

namespace FileContextCore.Serializer
{
    class XMLSerializer : ISerializer
    {
        private IEntityType entityType;
        private string[] propertyKeys;
        private readonly Type[] typeList;

        public XMLSerializer(IEntityType _entityType)
        {
            entityType = _entityType;
            propertyKeys = entityType.GetProperties().Select(p => p.Relational().ColumnName).ToArray();
            typeList = entityType.GetProperties().Select(p => p.ClrType).ToArray();
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList)
        {
            if (list != "")
            {
                XDocument xml = XDocument.Parse(list);
                XElement array = (XElement)xml.FirstNode;

                XElement current = (XElement)array.FirstNode;

                while (current != null)
                {
                    TKey key = (TKey)current.Attributes().FirstOrDefault(attr => attr.Name == "Key")?.Value.Deserialize(typeof(TKey));
                    Dictionary<string, string> values = current.Nodes().Select(x => (XElement)x).ToDictionary(x => x.Name.LocalName, x => x.Value);

                    List<object> value = new List<object>();

                    for (int i = 0; i < propertyKeys.Length; i++)
                    {
                        value.Add(values[propertyKeys[i]].Deserialize(typeList[i]));
                    }

                    newList.Add(key, value.ToArray());

                    current = (XElement)current.NextNode;
                }
            }

            return newList;
        }

        public string Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            StringWriter sw = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings() {
                Indent = true,
                OmitXmlDeclaration = true
            });

            writer.WriteStartDocument();
            writer.WriteStartElement("ArrayOfContent");

            string[] nameParts = entityType.Name.Split('.');
            string name = nameParts[nameParts.Length - 1].Replace("<", "").Replace(">", "");

            WriteList(writer, name, list);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            return sw.ToString();
        }

        private void WriteList<TKey>(XmlWriter writer, string name, Dictionary<TKey, object[]> list)
        {
            foreach (KeyValuePair<TKey, object[]> val in list)
            {
                writer.WriteStartElement(name);
                writer.WriteAttributeString("Key", val.Key.Serialize());

                for (int i = 0; i < propertyKeys.Length; i++)
                {
                    writer.WriteStartElement(propertyKeys[i]);
                    writer.WriteValue(val.Value[i].Serialize());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}
