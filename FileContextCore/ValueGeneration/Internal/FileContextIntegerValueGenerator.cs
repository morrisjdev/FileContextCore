// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FileContextCore.Infrastructure.Internal;
using FileContextCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace FileContextCore.ValueGeneration.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextIntegerValueGenerator<TValue> : ValueGenerator<TValue>
    {
        private long current = 0;
        private IProperty property;
        private readonly IFileContextStoreCache storeCache;
		private FileContextIntegerValueGeneratorCache idCache;

        private void ComputeLast()
        {
            if (idCache.LastIds.TryGetValue(property.DeclaringEntityType.Name, out Dictionary<string, long> props))
            {
                if (props.TryGetValue(property.Name, out long last))
                {
                    current = last;
                }
            }
        }

        public FileContextIntegerValueGenerator(IProperty _property, IFileContextStoreCache _storeCache, FileContextIntegerValueGeneratorCache _idCache, FileContextOptionsExtension options)
        {
            property = _property;
            storeCache = _storeCache;
			idCache = _idCache;

            if (idCache.LastIds.ContainsKey(property.DeclaringEntityType.Name))
            {
                ComputeLast();
            }
            else
            {
				storeCache.GetStore(options).GetTables(property.DeclaringEntityType);
				ComputeLast();
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override TValue Next(EntityEntry entry)
        {
            TValue next = (TValue)Convert.ChangeType(Interlocked.Increment(ref current), typeof(TValue), CultureInfo.InvariantCulture);

            if (idCache.LastIds.TryGetValue(property.DeclaringEntityType.Name, out Dictionary<string, long> props))
            {
                if (props.ContainsKey(property.Name))
                {
                    props[property.Name] = current;
                }
            }

            return next;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override bool GeneratesTemporaryValues => false;
    }
}
