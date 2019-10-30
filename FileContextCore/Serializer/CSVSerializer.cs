using CsvHelper;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace FileContextCore.Serializer
{
    class CSVSerializer<T> : ISerializer
    {
        private IEntityType entityType;
        private readonly IPrincipalKeyValueFactory<T> _keyValueFactory;
        private string[] propertyKeys;
        private readonly Type[] typeList;

        public CSVSerializer(IEntityType _entityType, IPrincipalKeyValueFactory<T> _keyValueFactory)
        {
            entityType = _entityType;
            this._keyValueFactory = _keyValueFactory;
            propertyKeys = entityType.GetProperties().Select(p => p.GetColumnName()).ToArray();
            typeList = entityType.GetProperties().Select(p => p.GetValueConverter()?.ProviderClrType ?? p.ClrType).ToArray();
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList)
        {
            if (string.IsNullOrEmpty(list))
            {
                return new Dictionary<TKey, object[]>();
            }

            TextReader tr = new StringReader(list);
            CsvReader reader = new CsvReader(tr);

            reader.Read();
            reader.ReadHeader();

            while (reader.Read())
            {
                List<object> value = new List<object>();

                for (int i = 0; i < propertyKeys.Length; i++)
                {
                    object val = reader.GetField(propertyKeys[i]).Deserialize(typeList[i]);
                    value.Add(val);
                }

                TKey key = SerializerHelper.GetKey<TKey, T>(_keyValueFactory, entityType,
                    propertyName => reader.GetField(propertyName));

                newList.Add(key, value.ToArray());
            }

            return newList;
        }

        public string Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            StringWriter sw = new StringWriter();
            CsvWriter writer = new CsvWriter(sw);

            for (int i = 0; i < propertyKeys.Length; i++)
            {
                writer.WriteField(propertyKeys[i]);
            }

            writer.NextRecord();

            foreach (KeyValuePair<TKey, object[]> val in list)
            {
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
