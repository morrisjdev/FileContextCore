using FileContextCore.Helper;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileContextCore.CombinedManager
{
    public class ExcelManager : ICombinedManager
    {
        private ExcelPackage package;

        public ExcelManager(string password = "")
        {
            if (password != "")
            {
                package = new ExcelPackage(GetFilePath("data.xlsx"), password);
            }
            else
            {
                package = new ExcelPackage(GetFilePath("data.xlsx"));
            }
        }

        FileInfo GetFilePath(string fileName)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "appdata");
            Directory.CreateDirectory(path);

            return new FileInfo(Path.Combine(path, fileName));
        }

        public IList GetItems(Type t)
        {
            ExcelWorksheet ws = package.Workbook.Worksheets[t.Name];

            if(ws != null)
            {
                List<PropertyInfo> props = t.GetRuntimeProperties().Where(x => !x.SetMethod.IsVirtual).ToList();
                Dictionary<int, PropertyInfo> properties = new Dictionary<int, PropertyInfo>();
                
                for(int i = 0; i < ws.Dimension.Columns; i++)
                {
                    properties.Add(i + 1, props.FirstOrDefault(x => x.Name == (string)ws.Cells[1, i + 1].Value));
                }

                IList result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));

                for (int i = 1; i < ws.Dimension.Rows; i++)
                {
                    object item = Activator.CreateInstance(t);
                    
                    foreach(KeyValuePair<int, PropertyInfo> prop in properties)
                    {
                        if(prop.Value.PropertyType == typeof(TimeSpan))
                        {
                            prop.Value.SetValue(item, TimeSpan.Parse((string)ws.Cells[i + 1, prop.Key].Value));
                        }
                        else
                        {
                            prop.Value.SetValue(item, Convert.ChangeType(ws.Cells[i + 1, prop.Key].Value, prop.Value.PropertyType));
                        }
                    }

                    result.Add(item);
                }

                return result;
            }
            else
            {
                ws = package.Workbook.Worksheets.Add(t.Name);

                PropertyInfo[] props = t.GetRuntimeProperties().Where(x => !x.SetMethod.IsVirtual).ToArray();

                for (int i = 0; i < props.Count(); i++)
                {
                    PropertyInfo pi = props[i];
                    ws.Cells[1, i + 1].Value = pi.Name;
                    ws.Column(i + 1).AutoFit();
                }

                ws.View.FreezePanes(2, 1);

                package.Save();
                return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
            }            
        }

        public void SaveItems(IList list)
        {
            Type t = list.GetType().GenericTypeArguments[0];
            PropertyInfo[] props = t.GetRuntimeProperties().Where(x => !x.SetMethod.IsVirtual).ToArray();

            ExcelWorksheet ws = package.Workbook.Worksheets[t.Name];

            for (int i = 0; i < list.Count; i++)
            {
                object item = list[i];

                for (int x = 0; x < props.Count(); x++)
                {
                    PropertyInfo pi = props[x];

                    ws.SetValue(i + 2, x + 1, pi.GetValue(item).ToString());
                }
            }

            for (int i = 0; i < ws.Dimension.Columns; i++)
            {
                ws.Column(i + 1).AutoFit();
            }

            package.Save();
        }
    }
}
