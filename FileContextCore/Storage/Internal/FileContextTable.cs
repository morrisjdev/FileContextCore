// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FileContextCore.FileManager;
using FileContextCore.Infrastructure.Internal;
using FileContextCore.Internal;
using FileContextCore.Serializer;
using FileContextCore.Utilities;
using FileContextCore.ValueGeneration.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;

namespace FileContextCore.Storage.Internal
{

    public class FileContextTable<TKey> : IFileContextTable
    {
        // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
        private readonly Microsoft.EntityFrameworkCore.ChangeTracking.Internal.IPrincipalKeyValueFactory<TKey> _keyValueFactory;
        private readonly bool _sensitiveLoggingEnabled;
        private readonly IEntityType _entityType;
        private readonly IFileContextScopedOptions _options;
        private readonly Dictionary<TKey, object[]> _rows;

        private IFileManager fileManager;
        private ISerializer<TKey> serializer;
        private string filetype;

        private Dictionary<int, IFileContextIntegerValueGenerator> _integerGenerators;

    
        public FileContextTable(
            // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
            [NotNull] Microsoft.EntityFrameworkCore.ChangeTracking.Internal.IPrincipalKeyValueFactory<TKey> keyValueFactory,
            bool sensitiveLoggingEnabled,
            IEntityType entityType,
            IFileContextScopedOptions options)
        {
            _keyValueFactory = keyValueFactory;
            _sensitiveLoggingEnabled = sensitiveLoggingEnabled;
            _entityType = entityType;
            _options = options;

            _rows = Init();
        }

    
        public virtual FileContextIntegerValueGenerator<TProperty> GetIntegerValueGenerator<TProperty>(IProperty property)
        {
            if (_integerGenerators == null)
            {
                _integerGenerators = new Dictionary<int, IFileContextIntegerValueGenerator>();
            }

            // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
            var propertyIndex = Microsoft.EntityFrameworkCore.Metadata.Internal.PropertyBaseExtensions.GetIndex(property);
            if (!_integerGenerators.TryGetValue(propertyIndex, out var generator))
            {
                generator = new FileContextIntegerValueGenerator<TProperty>(propertyIndex);
                _integerGenerators[propertyIndex] = generator;

                foreach (var row in _rows.Values)
                {
                    generator.Bump(row);
                }
            }

            return (FileContextIntegerValueGenerator<TProperty>)generator;
        }

    
        public virtual IReadOnlyList<object[]> SnapshotRows()
            => _rows.Values.ToList();

        private static List<ValueComparer> GetStructuralComparers(IEnumerable<IProperty> properties)
            => properties.Select(GetStructuralComparer).ToList();

        private static ValueComparer GetStructuralComparer(IProperty p)
            => p.GetStructuralValueComparer() ?? p.FindTypeMapping()?.StructuralComparer;

    
        public virtual void Create(IUpdateEntry entry)
        {
            var row = entry.EntityType.GetProperties()
                .Select(p => SnapshotValue(p, GetStructuralComparer(p), entry))
                .ToArray();

            _rows.Add(CreateKey(entry), row);

            BumpValueGenerators(row);
        }

    
        public virtual void Delete(IUpdateEntry entry)
        {
            var key = CreateKey(entry);

            if (_rows.ContainsKey(key))
            {
                var properties = entry.EntityType.GetProperties().ToList();
                var concurrencyConflicts = new Dictionary<IProperty, object>();

                for (var index = 0; index < properties.Count; index++)
                {
                    IsConcurrencyConflict(entry, properties[index], _rows[key][index], concurrencyConflicts);
                }

                if (concurrencyConflicts.Count > 0)
                {
                    ThrowUpdateConcurrencyException(entry, concurrencyConflicts);
                }

                _rows.Remove(key);
            }
            else
            {
                throw new DbUpdateConcurrencyException(FileContextStrings.UpdateConcurrencyException, new[] { entry });
            }
        }

        private static bool IsConcurrencyConflict(
            IUpdateEntry entry,
            IProperty property,
            object rowValue,
            Dictionary<IProperty, object> concurrencyConflicts)
        {
            if (property.IsConcurrencyToken
                && !StructuralComparisons.StructuralEqualityComparer.Equals(
                    rowValue,
                    entry.GetOriginalValue(property)))
            {
                concurrencyConflicts.Add(property, rowValue);

                return true;
            }

            return false;
        }

    
        public virtual void Update(IUpdateEntry entry)
        {
            var key = CreateKey(entry);

            if (_rows.ContainsKey(key))
            {
                var properties = entry.EntityType.GetProperties().ToList();
                var comparers = GetStructuralComparers(properties);
                var valueBuffer = new object[properties.Count];
                var concurrencyConflicts = new Dictionary<IProperty, object>();

                for (var index = 0; index < valueBuffer.Length; index++)
                {
                    if (IsConcurrencyConflict(entry, properties[index], _rows[key][index], concurrencyConflicts))
                    {
                        continue;
                    }

                    valueBuffer[index] = entry.IsModified(properties[index])
                        ? SnapshotValue(properties[index], comparers[index], entry)
                        : _rows[key][index];
                }

                if (concurrencyConflicts.Count > 0)
                {
                    ThrowUpdateConcurrencyException(entry, concurrencyConflicts);
                }

                _rows[key] = valueBuffer;

                BumpValueGenerators(valueBuffer);
            }
            else
            {
                throw new DbUpdateConcurrencyException(FileContextStrings.UpdateConcurrencyException, new[] { entry });
            }
        }

        private void BumpValueGenerators(object[] row)
        {
            if (_integerGenerators != null)
            {
                foreach (var generator in _integerGenerators.Values)
                {
                    generator.Bump(row);
                }
            }
        }

        // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
        private TKey CreateKey(IUpdateEntry entry)
            => _keyValueFactory.CreateFromCurrentValues((Microsoft.EntityFrameworkCore.ChangeTracking.Internal.InternalEntityEntry)entry);

        private static object SnapshotValue(IProperty property, ValueComparer comparer, IUpdateEntry entry)
            => SnapshotValue(comparer, entry.GetCurrentValue(property));

        private static object SnapshotValue(ValueComparer comparer, object value)
            => comparer == null ? value : comparer.Snapshot(value);

        public void Save()
        {
            UpdateMethod(_rows);
        }

        /// <summary>
        ///     Throws an exception indicating that concurrency conflicts were detected.
        /// </summary>
        /// <param name="entry"> The update entry which resulted in the conflict(s). </param>
        /// <param name="concurrencyConflicts"> The conflicting properties with their associated database values. </param>
        protected virtual void ThrowUpdateConcurrencyException([NotNull] IUpdateEntry entry, [NotNull] Dictionary<IProperty, object> concurrencyConflicts)
        {
            Check.NotNull(entry, nameof(entry));
            Check.NotNull(concurrencyConflicts, nameof(concurrencyConflicts));

            if (_sensitiveLoggingEnabled)
            {
                throw new DbUpdateConcurrencyException(
                    FileContextStrings.UpdateConcurrencyTokenExceptionSensitive(
                        entry.EntityType.DisplayName(),
                        entry.BuildCurrentValuesString(entry.EntityType.FindPrimaryKey().Properties),
                        entry.BuildOriginalValuesString(concurrencyConflicts.Keys),
                        "{" + string.Join(", ", concurrencyConflicts.Select(c => c.Key.Name + ": " + Convert.ToString(c.Value, CultureInfo.InvariantCulture))) + "}"),
                    new[] { entry });
            }

            throw new DbUpdateConcurrencyException(
                FileContextStrings.UpdateConcurrencyTokenException(
                    entry.EntityType.DisplayName(),
                    concurrencyConflicts.Keys.Format()),
                new[] { entry });
        }

        private void InitSerializer()
        {
            if (_options.Serializer == "xml")
            {
                serializer = new XMLSerializer<TKey>();
            }
            else if (_options.Serializer == "bson")
            {
                serializer = new BSONSerializer<TKey>();
            }
            else if (_options.Serializer == "csv")
            {
                serializer = new CSVSerializer<TKey>();
            }
            else
            {
                serializer = new JSONSerializer<TKey>();
            }
            
            serializer.Initialize(_entityType, _keyValueFactory);
        }

        private Action<Dictionary<TKey, object[]>> UpdateMethod;

        private void InitFileManager()
        {
            string fmgr = _options.FileManager ?? "default";

            if (fmgr.Length >= 9 && fmgr.Substring(0, 9) == "encrypted")
            {
                string password = "";

                if (fmgr.Length > 9)
                {
                    password = fmgr.Substring(10);
                }

                fileManager = new EncryptedFileManager(_entityType, filetype, password, _options.DatabaseName, _options.Location);
            }
            else if (fmgr == "private")
            {
                fileManager = new PrivateFileManager(_entityType, filetype, _options.DatabaseName, _options.Location);
            }
            else
            {
                fileManager = new DefaultFileManager(_entityType, filetype, _options.DatabaseName, _options.Location);
            }
        }

        private Dictionary<TKey, object[]> Init()
        {
            filetype = _options.Serializer ?? "json";

            if (filetype.Length >= 5 && filetype.Substring(0, 5) == "excel")
            {
                return InitExcel(filetype);
            }

            InitSerializer();
            InitFileManager();

            UpdateMethod = new Action<Dictionary<TKey, object[]>>((list) =>
            {
                string cnt = serializer.Serialize(ConvertToProvider(list));
                fileManager.SaveContent(cnt);
            });

            string content = fileManager.LoadContent();
            Dictionary<TKey, object[]> newList = new Dictionary<TKey, object[]>(_keyValueFactory.EqualityComparer);
            Dictionary<TKey, object[]> result = ConvertFromProvider(serializer.Deserialize(content, newList));
            return result;
        }

        private Dictionary<TKey, object[]> InitExcel(string filetype)
        {
            string password = "";

            if (filetype.Length > 5)
            {
                password = filetype.Substring(6);
            }

            EXCELSerializer<TKey> excel = new EXCELSerializer<TKey>(_entityType, password, _options.DatabaseName, _options.Location, _keyValueFactory);

            UpdateMethod = new Action<Dictionary<TKey, object[]>>((list) =>
            {
                excel.Serialize(ConvertToProvider(list));
            });

            Dictionary<TKey, object[]> newlist = new Dictionary<TKey, object[]>(_keyValueFactory.EqualityComparer);
            Dictionary<TKey, object[]> excelresult = excel.Deserialize(newlist);
            return excelresult;
        }

        private Dictionary<TKey, object[]> ApplyValueConverter(Dictionary<TKey, object[]> list, Func<ValueConverter, Func<object, object>> conversionFunc)
        {
            var result = new Dictionary<TKey, object[]>(_keyValueFactory.EqualityComparer);
            var converters = _entityType.GetProperties().Select(p => p.GetValueConverter()).ToArray();
            foreach (var keyValuePair in list)
            {
                result[keyValuePair.Key] = keyValuePair.Value.Select((value, index) =>
                {
                    var converter = converters[index];
                    return converter == null ? value : conversionFunc(converter)(value);
                }).ToArray();
            }
            return result;
        }

        private Dictionary<TKey, object[]> ConvertToProvider(Dictionary<TKey, object[]> list)
        {
            return ApplyValueConverter(list, converter => converter.ConvertToProvider);
        }

        private Dictionary<TKey, object[]> ConvertFromProvider(Dictionary<TKey, object[]> list)
        {
            return ApplyValueConverter(list, converter => converter.ConvertFromProvider);
        }
    }
}
