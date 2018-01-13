// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using FileContextCore.Extensions.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update;

namespace FileContextCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextStore : IFileContextStore
    {
        private readonly IFileContextTableFactory _tableFactory;

        private readonly object _lock = new object();

        private LazyRef<Dictionary<IEntityType, IFileContextTable>> _tables = CreateTables();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public FileContextStore([NotNull] IFileContextTableFactory tableFactory)
        {
            _tableFactory = tableFactory;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool EnsureCreated(IModel model)
        {
            lock (_lock)
            {
                bool returnValue = !_tables.HasValue;
                
                // ReSharper disable once AssignmentIsFullyDiscarded
                _ = _tables.Value;

                return returnValue;
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool Clear()
        {
            lock (_lock)
            {
                if (!_tables.HasValue)
                {
                    return false;
                }

                _tables = CreateTables();
                return true;
            }
        }

        private static LazyRef<Dictionary<IEntityType, IFileContextTable>> CreateTables()
        {
            return new LazyRef<Dictionary<IEntityType, IFileContextTable>>(
                () => new Dictionary<IEntityType, IFileContextTable>());
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IReadOnlyList<FileContextTableSnapshot> GetTables(IEntityType entityType)
        {
            List<FileContextTableSnapshot> data = new List<FileContextTableSnapshot>();
            lock (_lock)
            {
                foreach (IEntityType et in entityType.GetConcreteTypesInHierarchy())
                {

                    if (!_tables.Value.TryGetValue(et, out IFileContextTable table))
                    {
                        _tables.Value.Add(entityType, table = _tableFactory.Create(entityType));
                    }

                    data.Add(new FileContextTableSnapshot(et, table.SnapshotRows()));
                }
            }

            return data;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual int ExecuteTransaction(
            IEnumerable<IUpdateEntry> entries,
            IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
        {
            int rowsAffected = 0;

            lock (_lock)
            {
                foreach (IUpdateEntry entry in entries)
                {
                    IEntityType entityType = entry.EntityType;

                    Debug.Assert(!entityType.IsAbstract());

                    if (!_tables.Value.TryGetValue(entityType, out IFileContextTable table))
                    {
                        _tables.Value.Add(entityType, table = _tableFactory.Create(entityType));
                    }

                    switch (entry.EntityState)
                    {
                        case EntityState.Added:
                            table.Create(entry);
                            break;
                        case EntityState.Deleted:
                            table.Delete(entry);
                            break;
                        case EntityState.Modified:
                            table.Update(entry);
                            break;
                    }

                    rowsAffected++;
                }

                foreach (KeyValuePair<IEntityType, IFileContextTable> table in _tables.Value)
                {
                    table.Value.Save();
                }
            }

            updateLogger.ChangesSaved(entries, rowsAffected);

            return rowsAffected;
        }
    }
}
