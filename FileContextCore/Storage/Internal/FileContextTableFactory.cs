// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using FileContextCore.Infrastructure.Internal;
using FileContextCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace FileContextCore.Storage.Internal
{

    public class FileContextTableFactory
        // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
        : Microsoft.EntityFrameworkCore.ChangeTracking.Internal.IdentityMapFactoryFactoryBase, IFileContextTableFactory
    {
        private readonly IFileContextScopedOptions _options;
        private readonly bool _sensitiveLoggingEnabled;

        private readonly ConcurrentDictionary<IKey, Func<IFileContextTable>> _factories
            = new ConcurrentDictionary<IKey, Func<IFileContextTable>>();

    
        public FileContextTableFactory([NotNull] ILoggingOptions loggingOptions, [NotNull] IFileContextScopedOptions options)
        {
            _options = options;
            Check.NotNull(loggingOptions, nameof(loggingOptions));

            _sensitiveLoggingEnabled = loggingOptions.IsSensitiveDataLoggingEnabled;
        }

    
        public virtual IFileContextTable Create(IEntityType entityType)
            => _factories.GetOrAdd(entityType.FindPrimaryKey(), Create)();

        private Func<IFileContextTable> Create([NotNull] IKey key)
            => (Func<IFileContextTable>)typeof(FileContextTableFactory).GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateFactory))
                .MakeGenericMethod(GetKeyType(key))
                .Invoke(null, new object[] { key, key.DeclaringEntityType, _sensitiveLoggingEnabled, _options });

        [UsedImplicitly]
        private static Func<IFileContextTable> CreateFactory<TKey>(IKey key, IEntityType entityType, bool sensitiveLoggingEnabled, IFileContextScopedOptions options)
            => () => new FileContextTable<TKey>(
                // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
                Microsoft.EntityFrameworkCore.Metadata.Internal.KeyExtensions.GetPrincipalKeyValueFactory<TKey>(key),
                sensitiveLoggingEnabled,
                entityType,
                options);
    }
}
