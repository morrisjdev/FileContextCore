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
using CsvHelper;
using System.IO;
using CsvHelper.Configuration;

namespace FileContextCore.Serializer
{
    public class CSVSerializer : ISerializer
    {
        private string delimiter;

        public CSVSerializer(string _delimiter = ",")
        {
            delimiter = _delimiter;
        }

        public string FileType { get { return "csv"; } }

        public List<T> DeserializeList<T>(string list)
        {
            if(list != "")
            {
                PropertyInfo[] properties = typeof(T).GetPropertiesForSerialize();

                CsvClassMap map = new DefaultCsvClassMap<T>();

                foreach (PropertyInfo pi in properties)
                {
                    CsvPropertyMap propMap = new CsvPropertyMap(pi);
                    propMap.Name(pi.Name);
                    propMap.Index(0);
                    map.PropertyMaps.Add(propMap);
                }

                TextReader tr = new StringReader(list);
                CsvReader reader = new CsvReader(tr);
                reader.Configuration.Delimiter = delimiter;
                reader.Configuration.RegisterClassMap(map);

                List<T> result = new List<T>();

                foreach (T record in reader.GetRecords<T>())
                {
                    result.Add(record);
                }

                return result;
            }

            return new List<T>();
        }

        public string SerializeList<T>(List<T> list)
        {
            Type t = typeof(T);
            PropertyInfo[] properties = t.GetPropertiesForSerialize();

            CsvClassMap map = new DefaultCsvClassMap<T>();

            foreach (PropertyInfo pi in properties)
            {
                CsvPropertyMap propMap = new CsvPropertyMap(pi);
                propMap.Name(pi.Name);
                propMap.Index(0);
                map.PropertyMaps.Add(propMap);
            }

            StringWriter sw = new StringWriter();
            CsvWriter writer = new CsvWriter(sw);
            writer.Configuration.Delimiter = delimiter;
            writer.Configuration.RegisterClassMap(map);

            writer.WriteRecords(list);

            return sw.ToString();
        }

        public T Deserialize<T>(string obj)
        {
            return DeserializeList<T>(obj).FirstOrDefault();
        }

        public string Serialize<T>(T obj)
        {
            List<T> list = new List<T>
            {
                obj
            };
            return SerializeList<T>(list);
        }
    }
}
