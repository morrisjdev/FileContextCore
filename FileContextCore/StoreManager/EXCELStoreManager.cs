using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileContextCore.Infrastructure.Internal;
using FileContextCore.Serializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using OfficeOpenXml;

namespace FileContextCore.StoreManager
{
    public class EXCELStoreManager : IStoreManager
    {
        private IEntityType _entityType;
        private string[] _propertyKeys;
        private Type[] _typeList;
        private int[] _propertyColumnIndices;
        private IFileContextScopedOptions _options;
        private object _keyValueFactory;

        private static Dictionary<string, ExcelPackage> _packages = new Dictionary<string, ExcelPackage>();
        private ExcelPackage _package;
        private ExcelWorksheet _worksheet;

        FileInfo GetFilePath()
        {
            string folder = string.IsNullOrEmpty(_options.Location)
                ? Path.Combine(AppContext.BaseDirectory, "appdata")
                : _options.Location;

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return new FileInfo(Path.Combine(folder, $"{_options.DatabaseName}.xlsx"));
        }

        public void Initialize(IFileContextScopedOptions options, IEntityType entityType, object keyValueFactory)
        {
            _options = options;
            _entityType = entityType;
            _propertyKeys = entityType.GetProperties().Select(p => p.GetColumnName()).ToArray();
            _propertyColumnIndices = new int[_propertyKeys.Length];
            _typeList = entityType.GetProperties().Select(p => p.GetValueConverter()?.ProviderClrType ?? p.ClrType)
                .ToArray();
            _keyValueFactory = keyValueFactory;

            if (!_packages.ContainsKey(options.DatabaseName))
            {
                if (!String.IsNullOrEmpty(options.Password))
                {
                    _package = new ExcelPackage(GetFilePath(), options.Password);
                    _packages.Add(options.DatabaseName, _package);
                }
                else
                {
                    _package = new ExcelPackage(GetFilePath());
                    _packages.Add(options.DatabaseName, _package);
                }
            }
            else
            {
                _package = _packages[options.DatabaseName];
            }
            
            string name = entityType.GetTableName();
            _worksheet = _package.Workbook.Worksheets[name];

            if (_worksheet == null)
            {
                _worksheet = _package.Workbook.Worksheets.Add(name);
                _worksheet.Column(1).AutoFit();

                for (int i = 0; i < _propertyKeys.Length; i++)
                {
                    _worksheet.Cells[1, i + 1].Value = _propertyKeys[i];
                    _propertyColumnIndices[i] = i + 1;
                    _worksheet.Column(i + 1).AutoFit();
                }

                _worksheet.View.FreezePanes(2, 1);

                if (!String.IsNullOrEmpty(options.Password))
                {
                    _package.Save(options.Password);
                }
                else
                {
                    _package.Save();
                }
            }
            else
            {
                for (int i = 0; i < _propertyKeys.Length; i++)
                {
                    for (int x = 0; x < _worksheet.Dimension.Columns; x++)
                    {
                        string val = _worksheet.Cells[1, x + 1].GetValue<string>();
                        if (_propertyKeys[i].Equals(val, StringComparison.InvariantCultureIgnoreCase))
                        {
                            _propertyColumnIndices[i] = x + 1;
                            break;
                        }
                    }
                }
            }
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(Dictionary<TKey, object[]> newList)
        {
            for (int y = 2; y <= _worksheet.Dimension.Rows; y++)
            {
                List<object> value = new List<object>();

                for (int x = 0; x < _propertyKeys.Length; x++)
                {
                    object val = _worksheet.Cells[y, _propertyColumnIndices[x]].GetValue<string>()
                        .Deserialize(_typeList[x]);
                    value.Add(val);
                }

                TKey key = SerializerHelper.GetKey<TKey>(_keyValueFactory, _entityType, propertyName =>
                    _worksheet.Cells[y, _propertyColumnIndices[_propertyKeys.IndexOf(propertyName)]]
                        .GetValue<string>());

                newList.Add(key, value.ToArray());
            }

            return newList;
        }

        public void Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            FillRows(list);

            for (int x = 0; x < _worksheet.Dimension.Columns; x++)
            {
                _worksheet.Column(x + 1).AutoFit();
            }

            DeleteUnusedRows(list.Count + 1);

            if (!String.IsNullOrEmpty(_options.Password))
            {
                _package.Save(_options.Password);
            }
            else
            {
                _package.Save();
            }
        }

        private void FillRows<TKey>(Dictionary<TKey, object[]> list)
        {
            int y = 2;

            foreach (KeyValuePair<TKey, object[]> val in list)
            {
                for (int x = 0; x < _propertyKeys.Length; x++)
                {
                    _worksheet.SetValue(y, x + 1, val.Value[x].Serialize());
                }

                y++;
            }
        }

        private void DeleteUnusedRows(int lastRow)
        {
            if (_worksheet.Dimension.Rows >= lastRow)
            {
                int count = _worksheet.Dimension.Rows - lastRow;
                _worksheet.DeleteRow(lastRow + 1, count);
            }
        }
    }
}