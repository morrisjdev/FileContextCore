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
        private string password = "";

        private string fileName = "";

        private string folder = Path.Combine(AppContext.BaseDirectory, "appdata");

        public ExcelManager(string _password = "", string _fileName = "data.xlsx", string _folder = "")
        {
            password = _password;
            fileName = _fileName;

            if(_folder != "")
            {
                folder = _folder;
            }
        }

        FileInfo GetFilePath(string fileName)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return new FileInfo(Path.Combine(folder, fileName));
        }

        public List<T> GetItems<T>()
        {
            Type t = typeof(T);
            ExcelPackage package;

            if (password != "")
            {
                package = new ExcelPackage(GetFilePath(fileName), password);
            }
            else
            {
                package = new ExcelPackage(GetFilePath(fileName));
            }

            ExcelWorksheet ws = package.Workbook.Worksheets[t.Name];

            if (ws != null)
            {
                List<PropertyInfo> props = t.GetRuntimeProperties().Where(x => !x.SetMethod.IsVirtual).ToList();
                Dictionary<int, PropertyInfo> properties = new Dictionary<int, PropertyInfo>();

                for (int i = 0; i < ws.Dimension.Columns; i++)
                {
                    properties.Add(i + 1, props.FirstOrDefault(x => x.Name == (string)ws.Cells[1, i + 1].Value));
                }

                List<T> result = new List<T>();

                for (int i = 1; i < ws.Dimension.Rows; i++)
                {
                    T item = (T)Activator.CreateInstance(t);

                    foreach (KeyValuePair<int, PropertyInfo> prop in properties)
                    {
                        Type type = prop.Value.PropertyType;
                        if (type == typeof(TimeSpan))
                        {
                            prop.Value.SetValue(item, TimeSpan.Parse((string)ws.Cells[i + 1, prop.Key].Value));
                        }
                        else if (type == typeof(Guid))
                        {
                            prop.Value.SetValue(item, Guid.Parse((string)ws.Cells[i + 1, prop.Key].Value));
                        }
                        else
                        {
                            prop.Value.SetValue(item, Convert.ChangeType(ws.Cells[i + 1, prop.Key].Value, type));
                        }
                    }

                    result.Add(item);
                }

                package.Dispose();

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

                if (password != "")
                {
                    package.Save(password);
                }
                else
                {
                    package.Save();
                }

                package.Dispose();

                return new List<T>();
            }
        }

        public void SaveItems<T>(List<T> list)
        {
            ExcelPackage package;

            if (password != "")
            {
                package = new ExcelPackage(GetFilePath(fileName), password);
            }
            else
            {
                package = new ExcelPackage(GetFilePath(fileName));
            }

            Type t = list.GetType().GenericTypeArguments[0];
            PropertyInfo[] props = t.GetRuntimeProperties().Where(x => !x.SetMethod.IsVirtual).ToArray();

            ExcelWorksheet ws = package.Workbook.Worksheets[t.Name];

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];

                for (int x = 0; x < props.Count(); x++)
                {
                    PropertyInfo pi = props[x];

                    ws.SetValue(i + 2, x + 1, pi.GetValue(item).ToString());
                }
            }

            for (int y = 0; y < ws.Dimension.Columns; y++)
            {
                ws.Column(y + 1).AutoFit();
            }

            if (password != "")
            {
                package.Save(password);
            }
            else
            {
                package.Save();
            }

            package.Dispose();
        }

        public bool Clear()
        {
            bool result = false;
            ExcelPackage package;

            if (password != "")
            {
                package = new ExcelPackage(GetFilePath(fileName), password);
            }
            else
            {
                package = new ExcelPackage(GetFilePath(fileName));
            }

            if(package.Workbook.Worksheets.Count > 0)
            {
                for (int i = 0; i < package.Workbook.Worksheets.Count; i++)
                {
                    package.Workbook.Worksheets.Delete(i + 1);
                }

                if (password != "")
                {
                    package.Save(password);
                }
                else
                {
                    package.Save();
                }

                result = true;
            }   

            package.Dispose();

            return result;
        }

        public bool Exists()
        {
            bool result = false;
            ExcelPackage package;

            if (password != "")
            {
                package = new ExcelPackage(GetFilePath(fileName), password);
            }
            else
            {
                package = new ExcelPackage(GetFilePath(fileName));
            }

            result = package.Workbook.Worksheets.Count > 0;

            package.Dispose();

            return result;
        }
    }
}
