using CsvHelper;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace FileContextCore.Serializer
{
    class CSVSerializer : ISerializer
    {
        private IEntityType entityType;
        private string[] propertyKeys;
        private readonly Type[] typeList;

        public CSVSerializer(IEntityType _entityType)
        {
            entityType = _entityType;
            propertyKeys = entityType.GetProperties().Select(p => p.Relational().ColumnName).ToArray();
            typeList = entityType.GetProperties().Select(p => p.ClrType).ToArray();
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList)
        {
            TextReader tr = new StringReader(list);
            CsvReader reader = new CsvReader(tr);

            reader.Read();

            while (reader.Read())
            {
                TKey key = (TKey)reader.GetField(0).Deserialize(typeof(TKey));
                List<object> value = new List<object>();

                for (int i = 0; i < propertyKeys.Length; i++)
                {
                    object val = reader.GetField(i + 1).Deserialize(typeList[i]);
                    value.Add(val);
                }

                newList.Add(key, value.ToArray());
            }

            return newList;
        }

        public string Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            StringWriter sw = new StringWriter();
            CsvWriter writer = new CsvWriter(sw);

            writer.WriteField("Key");

            for (int i = 0; i < propertyKeys.Length; i++)
            {
                writer.WriteField(propertyKeys[i]);
            }

            writer.NextRecord();

            foreach (KeyValuePair<TKey, object[]> val in list)
            {
                writer.WriteField(val.Key.Serialize());

                for (int i = 0; i < propertyKeys.Length; i++)
                {
                    writer.WriteField(val.Value[i].Serialize());
                }

                writer.NextRecord();
            }

            return sw.ToString();
        }
    }
}
