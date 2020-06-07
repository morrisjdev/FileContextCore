using CsvHelper;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileContextCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FileContextCore.Serializer
{
    public class CSVSerializer : ISerializer
    {
        private IEntityType _entityType;
        private object _keyValueFactory;
        private string[] _propertyKeys;
        private Type[] _typeList;

        public void Initialize(IFileContextScopedOptions options, IEntityType entityType, object keyValueFactory)
        {
            _keyValueFactory = keyValueFactory;
            _entityType = entityType;
            _propertyKeys = entityType.GetProperties().Select(p => p.GetColumnName()).ToArray();
            _typeList = entityType.GetProperties().Select(p => p.GetValueConverter()?.ProviderClrType ?? p.ClrType).ToArray();
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList)
        {
            if (string.IsNullOrEmpty(list))
            {
                return new Dictionary<TKey, object[]>();
            }

            TextReader tr = new StringReader(list);
            CsvReader reader = new CsvReader(tr, CultureInfo.CurrentCulture);

            reader.Read();
            reader.ReadHeader();

            while (reader.Read())
            {
                List<object> value = new List<object>();

                for (int i = 0; i < _propertyKeys.Length; i++)
                {
                    object val = reader.GetField(_propertyKeys[i]).Deserialize(_typeList[i]);
                    value.Add(val);
                }

                TKey key = SerializerHelper.GetKey<TKey>(_keyValueFactory, _entityType,
                    propertyName => reader.GetField(propertyName));

                newList.Add(key, value.ToArray());
            }

            return newList;
        }

        public string Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            StringWriter sw = new StringWriter();
            CsvWriter writer = new CsvWriter(sw, CultureInfo.CurrentCulture);

            for (int i = 0; i < _propertyKeys.Length; i++)
            {
                writer.WriteField(_propertyKeys[i]);
            }

            writer.NextRecord();

            foreach (KeyValuePair<TKey, object[]> val in list)
            {
                for (int i = 0; i < _propertyKeys.Length; i++)
                {
                    writer.WriteField(val.Value[i].Serialize());
                }

                writer.NextRecord();
            }

            return sw.ToString();
        }
        
        public string FileType => "csv";
    }
}
