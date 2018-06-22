// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using FileContextCore.Infrastructure.Internal;
using FileContextCore.Storage.Internal;
using FileContextCore.Utilities;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        //public static Dictionary<string, Dictionary<string, long>> LastIds = new Dictionary<string, Dictionary<string, long>>();
        private readonly IFileContextStoreCache storeCache;
		public readonly FileContextIntegerValueGeneratorCache idCache;
		private readonly FileContextOptionsExtension options;


		public FileContextIntegerValueGeneratorFactory(IFileContextStoreCache _storeCache, FileContextIntegerValueGeneratorCache _idCache, IDbContextOptions _contextOptions)
        {
            storeCache = _storeCache;
			idCache = _idCache;
			options = _contextOptions.Extensions.OfType<FileContextOptionsExtension>().First();
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
                return new FileContextIntegerValueGenerator<long>(property, storeCache, idCache, options);
            }

            if (type == typeof(int))
            {
                return new FileContextIntegerValueGenerator<int>(property, storeCache, idCache, options);
            }

            if (type == typeof(short))
            {
                return new FileContextIntegerValueGenerator<short>(property, storeCache, idCache, options);
            }

            if (type == typeof(byte))
            {
                return new FileContextIntegerValueGenerator<byte>(property, storeCache, idCache, options);
            }

            if (type == typeof(ulong))
            {
                return new FileContextIntegerValueGenerator<ulong>(property, storeCache, idCache, options);
            }

            if (type == typeof(uint))
            {
                return new FileContextIntegerValueGenerator<uint>(property, storeCache, idCache, options);
            }

            if (type == typeof(ushort))
            {
                return new FileContextIntegerValueGenerator<ushort>(property, storeCache, idCache, options);
            }

            if (type == typeof(sbyte))
            {
                return new FileContextIntegerValueGenerator<sbyte>(property, storeCache, idCache, options);
            }

            throw new ArgumentException(CoreStrings.InvalidValueGeneratorFactoryProperty(
                nameof(FileContextIntegerValueGeneratorFactory), property.Name, property.DeclaringEntityType.DisplayName()));
        }
    }
}
