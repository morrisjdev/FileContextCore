// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using FileContextCore.Storage.Internal;
using FileContextCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace FileContextCore.ValueGeneration.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextValueGeneratorSelector : ValueGeneratorSelector
    {
        public readonly FileContextIntegerValueGeneratorFactory _fileContextFactory;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public FileContextValueGeneratorSelector([NotNull] ValueGeneratorSelectorDependencies dependencies, IFileContextStoreCache _storeCache, FileContextIntegerValueGeneratorCache _idCache, IDbContextOptions contextOptions)
            : base(dependencies)
        {
            _fileContextFactory = new FileContextIntegerValueGeneratorFactory(_storeCache, _idCache, contextOptions);

        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(entityType, nameof(entityType));

            return property.ClrType.IsInteger()
                ? _fileContextFactory.Create(property)
                : base.Create(property, entityType);
        }
    }
}
