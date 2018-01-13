// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using FileContextCore.Storage.Internal;
using FileContextCore.Utilities;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace FileContextCore.ValueGeneration.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextIntegerValueGeneratorFactory : ValueGeneratorFactory
    {
        public static Dictionary<string, Dictionary<string, long>> LastIds = new Dictionary<string, Dictionary<string, long>>();
        private IFileContextStoreCache storeCache;

        public FileContextIntegerValueGeneratorFactory(IFileContextStoreCache _storeCache)
        {
            storeCache = _storeCache;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ValueGenerator Create(IProperty property)
        {
            Check.NotNull(property, nameof(property));

            Type type = property.ClrType.UnwrapNullableType().UnwrapEnumType();

            if (type == typeof(long))
            {
                return new FileContextIntegerValueGenerator<long>(property, storeCache);
            }

            if (type == typeof(int))
            {
                return new FileContextIntegerValueGenerator<int>(property, storeCache);
            }

            if (type == typeof(short))
            {
                return new FileContextIntegerValueGenerator<short>(property, storeCache);
            }

            if (type == typeof(byte))
            {
                return new FileContextIntegerValueGenerator<byte>(property, storeCache);
            }

            if (type == typeof(ulong))
            {
                return new FileContextIntegerValueGenerator<ulong>(property, storeCache);
            }

            if (type == typeof(uint))
            {
                return new FileContextIntegerValueGenerator<uint>(property, storeCache);
            }

            if (type == typeof(ushort))
            {
                return new FileContextIntegerValueGenerator<ushort>(property, storeCache);
            }

            if (type == typeof(sbyte))
            {
                return new FileContextIntegerValueGenerator<sbyte>(property, storeCache);
            }

            throw new ArgumentException(CoreStrings.InvalidValueGeneratorFactoryProperty(
                nameof(FileContextIntegerValueGeneratorFactory), property.Name, property.DeclaringEntityType.DisplayName()));
        }
    }
}
