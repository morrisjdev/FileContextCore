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

        public IList DeserializeList(string list, Type t)
        {
            if(list != "")
            {
                PropertyInfo[] properties = t.GetRuntimeProperties().Where(x => !x.SetMethod.IsVirtual).ToArray();

                CsvClassMap map = Activator.CreateInstance(typeof(DefaultCsvClassMap<>).MakeGenericType(t)) as CsvClassMap;

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

                IList result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));

                foreach (object record in reader.GetRecords(t))
                {
                    result.Add(record);
                }

                return result;
            }

            return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
        }

        public string SerializeList(IList list)
        {
            Type t = list.GetType().GenericTypeArguments[0];
            PropertyInfo[] properties = t.GetRuntimeProperties().Where(x => !x.SetMethod.IsVirtual).ToArray();

            CsvClassMap map = Activator.CreateInstance(typeof(DefaultCsvClassMap<>).MakeGenericType(t)) as CsvClassMap;

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

        public object Deserialize(string obj, Type t)
        {
            throw new NotSupportedException();
        }

        public string Serialize(object obj)
        {
            throw new NotSupportedException();
        }
    }
}
