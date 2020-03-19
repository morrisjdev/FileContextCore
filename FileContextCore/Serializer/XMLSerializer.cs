using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using FileContextCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace FileContextCore.Serializer
{
    public class XMLSerializer : ISerializer
    {
        private IEntityType _entityType;
        private object _keyValueFactory;
        private string[] _propertyKeys;
        private Type[] _typeList;

        public XMLSerializer() { }

        public void Initialize(IFileContextScopedOptions options, IEntityType entityType, object keyValueFactory)
        {
            _entityType = entityType;
            _keyValueFactory = keyValueFactory;
            _propertyKeys = _entityType.GetProperties().Select(p => p.GetColumnName()).ToArray();
            _typeList = _entityType.GetProperties().Select(p => p.GetValueConverter()?.ProviderClrType ?? p.ClrType).ToArray();
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

                    for (int i = 0; i < _propertyKeys.Length; i++)
                    {
                        value.Add(values[_propertyKeys[i]].Deserialize(_typeList[i]));
                    }

                    TKey key = SerializerHelper.GetKey<TKey>(_keyValueFactory, _entityType, propertyName => values[propertyName]);

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

            string[] nameParts = _entityType.Name.Split('.');
            string name = nameParts[nameParts.Length - 1].Replace("<", "").Replace(">", "");

            WriteList(writer, name, list);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            return sw.ToString();
        }

        public string FileType => "xml";

        private void WriteList<TKey>(XmlWriter writer, string name, Dictionary<TKey, object[]> list)
        {
            foreach (KeyValuePair<TKey, object[]> val in list)
            {
                writer.WriteStartElement(name);

                for (int i = 0; i < _propertyKeys.Length; i++)
                {
                    writer.WriteStartElement(_propertyKeys[i]);
                    writer.WriteValue(val.Value[i].Serialize());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}
