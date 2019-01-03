using Microsoft.EntityFrameworkCore.Metadata;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileContextCore.Serializer
{
    class EXCELSerializer
    {
        private IEntityType entityType;
        private string[] propertyKeys;
        private readonly Type[] typeList;
        private readonly string password;
        private readonly string databaseName;

        private static Dictionary<string, ExcelPackage> packages = new Dictionary<string, ExcelPackage>();
        private ExcelPackage package;
        private ExcelWorksheet worksheet;
        

        FileInfo GetFilePath()
        {
            string folder = Path.Combine(AppContext.BaseDirectory, "appdata", databaseName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return new FileInfo(Path.Combine(folder, "data.xlsx"));
        }

        public EXCELSerializer(IEntityType _entityType, string _password, string databaseName)
        {
            entityType = _entityType;
            propertyKeys = entityType.GetProperties().Select(p => p.Name).ToArray();
            typeList = entityType.GetProperties().Select(p => p.ClrType).ToArray();
            password = _password;
            this.databaseName = databaseName;

            if (!packages.ContainsKey(databaseName))
            {
                if (!String.IsNullOrEmpty(password))
                {
                    package = new ExcelPackage(GetFilePath(), password);
                    packages.Add(databaseName, package);
                }
                else
                {
                    package = new ExcelPackage(GetFilePath());
                    packages.Add(databaseName, package);
                }
            }
            else
            {
                package = packages[databaseName];
            }

            string[] nameParts = entityType.Name.Split('.');
            string name = nameParts[nameParts.Length - 1];

            worksheet = package.Workbook.Worksheets[name];

            if (worksheet == null)
            {
                worksheet = package.Workbook.Worksheets.Add(name);

                worksheet.Cells[1, 1].Value = "Key";
                worksheet.Column(1).AutoFit();

                for (int i = 0; i < propertyKeys.Length; i++)
                {
                    worksheet.Cells[1, i + 2].Value = propertyKeys[i];
                    worksheet.Column(i + 2).AutoFit();
                }

                worksheet.View.FreezePanes(2, 1);

                if (!String.IsNullOrEmpty(password))
                {
                    package.Save(password);
                }
                else
                {
                    package.Save();
                }
            }
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(Dictionary<TKey, object[]> newList)
        {
            for (int y = 2; y < worksheet.Dimension.Rows; y++)
            {
                TKey key = (TKey)worksheet.Cells[y, 1].GetValue<string>().Deserialize(typeof(TKey));
                List<object> value = new List<object>();

                for (int x = 0; x < propertyKeys.Length; x++)
                {
                    object val = worksheet.Cells[y, x + 2].GetValue<string>().Deserialize(typeList[x]);
                    value.Add(val);
                }

                newList.Add(key, value.ToArray());
            }

            return newList;
        }

        public void Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            int y = 2;

            foreach (KeyValuePair<TKey, object[]> val in list)
            {
                worksheet.SetValue(y, 1, val.Key.Serialize());

                for (int x = 0; x < propertyKeys.Length; x++)
                {
                    worksheet.SetValue(y, x + 2, val.Value[x].Serialize());
                }

                y++;
            }

            for (int x = 0; x < worksheet.Dimension.Columns; x++)
            {
                worksheet.Column(x + 1).AutoFit();
            }

            int lastRow = list.Count + 1;

            if (worksheet.Dimension.Rows > lastRow)
            {
                int count = worksheet.Dimension.Rows - lastRow;
                worksheet.DeleteRow(lastRow + 1, count);
            }

            if (!String.IsNullOrEmpty(password))
            {
                package.Save(password);
            }
            else
            {
                package.Save();
            }
        }
    }
}
