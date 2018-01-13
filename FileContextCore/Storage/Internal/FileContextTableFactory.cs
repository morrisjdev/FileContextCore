// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FileContextCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextTableFactory : IdentityMapFactoryFactoryBase, IFileContextTableFactory
    {    
        public FileContextTableFactory()
        {

        }

        private readonly ConcurrentDictionary<IKey, Func<IFileContextTable>> _factories
            = new ConcurrentDictionary<IKey, Func<IFileContextTable>>();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IFileContextTable Create(IEntityType entityType)
            => _factories.GetOrAdd(entityType.FindPrimaryKey(), Create)();

        private Func<IFileContextTable> Create([NotNull] IKey key)
            => (Func<IFileContextTable>)typeof(FileContextTableFactory).GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateFactory))
                .MakeGenericMethod(GetKeyType(key))
                .Invoke(null, new object[] { key });

        [UsedImplicitly]
        private static Func<IFileContextTable> CreateFactory<TKey>(IKey key)
            => () => new FileContextTable<TKey>(key.GetPrincipalKeyValueFactory<TKey>(), key.DeclaringEntityType);
    }
}
