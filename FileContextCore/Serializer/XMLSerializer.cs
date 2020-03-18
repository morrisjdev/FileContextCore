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
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace FileContextCore.Serializer
{
    class XMLSerializer<T> : ISerializer<T>
    {
        private IEntityType entityType;
        private IPrincipalKeyValueFactory<T> _keyValueFactory;
        private string[] propertyKeys;
        private Type[] typeList;

        public XMLSerializer() { }

        public void Initialize(IEntityType _entityType, IPrincipalKeyValueFactory<T> _keyValueFactory)
        {
            entityType = _entityType;
            this._keyValueFactory = _keyValueFactory;
            propertyKeys = entityType.GetProperties().Select(p => p.GetColumnName()).ToArray();
            typeList = entityType.GetProperties().Select(p => p.GetValueConverter()?.ProviderClrType ?? p.ClrType).ToArray();
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
                    Dictionary<string, string> values = current.Nodes().Select(x => (XElement)x).ToDictionary(x => x.Name.LocalName, x => x.Value);

                    List<object> value = new List<object>();

                    for (int i = 0; i < propertyKeys.Length; i++)
                    {
                        value.Add(values[propertyKeys[i]].Deserialize(typeList[i]));
                    }

                    TKey key = SerializerHelper.GetKey<TKey, T>(_keyValueFactory, entityType, propertyName => values[propertyName]);

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
