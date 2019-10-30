using Microsoft.EntityFrameworkCore.Metadata;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;

namespace FileContextCore.Serializer
{
    class EXCELSerializer<T>
    {
        private IEntityType entityType;
        private string[] propertyKeys;
        private readonly Type[] typeList;
        private readonly string password;
        private readonly string databaseName;
        private readonly string _location;
        private readonly IPrincipalKeyValueFactory<T> _keyValueFactory;

        private static Dictionary<string, ExcelPackage> packages = new Dictionary<string, ExcelPackage>();
        private ExcelPackage package;
        private ExcelWorksheet worksheet;
        

        FileInfo GetFilePath()
        {
            string folder = string.IsNullOrEmpty(_location)
                ? Path.Combine(AppContext.BaseDirectory, "appdata")
                : _location;

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return new FileInfo(Path.Combine(folder, $"{databaseName}.xlsx"));
        }

        public EXCELSerializer(IEntityType _entityType, string _password, string databaseName, string location, IPrincipalKeyValueFactory<T> _keyValueFactory)
        {
            entityType = _entityType;
            propertyKeys = entityType.GetProperties().Select(p => p.GetColumnName()).ToArray();
            typeList = entityType.GetProperties().Select(p => p.GetValueConverter()?.ProviderClrType ?? p.ClrType).ToArray();
            password = _password;
            this.databaseName = databaseName;
            _location = location;
            this._keyValueFactory = _keyValueFactory;

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
                worksheet.Column(1).AutoFit();

                for (int i = 0; i < propertyKeys.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = propertyKeys[i];
                    worksheet.Column(i + 1).AutoFit();
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
                List<object> value = new List<object>();

                for (int x = 0; x < propertyKeys.Length; x++)
                {
                    object val = worksheet.Cells[y, x + 1].GetValue<string>().Deserialize(typeList[x]);
                    value.Add(val);
                }

                TKey key = SerializerHelper.GetKey<TKey, T>(_keyValueFactory, entityType, propertyName => 
                    worksheet.Cells[y, propertyKeys.IndexOf(propertyName) + 1].GetValue<string>());

                newList.Add(key, value.ToArray());
            }

            return newList;
        }

        public void Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            FillRows(list);

            for (int x = 0; x < worksheet.Dimension.Columns; x++)
            {
                worksheet.Column(x + 1).AutoFit();
            }

            DeleteUnusedRows(list.Count + 1);

            if (!String.IsNullOrEmpty(password))
            {
                package.Save(password);
            }
            else
            {
                package.Save();
            }
        }

        private void FillRows<TKey>(Dictionary<TKey, object[]> list)
        {
            int y = 2;

            foreach (KeyValuePair<TKey, object[]> val in list)
            {
                for (int x = 0; x < propertyKeys.Length; x++)
                {
                    worksheet.SetValue(y, x + 1, val.Value[x].Serialize());
                }

                y++;
            }
        }

        private void DeleteUnusedRows(int lastRow)
        {
            if (worksheet.Dimension.Rows > lastRow)
            {
                int count = worksheet.Dimension.Rows - lastRow;
                worksheet.DeleteRow(lastRow + 1, count);
            }
        }
    }
}
