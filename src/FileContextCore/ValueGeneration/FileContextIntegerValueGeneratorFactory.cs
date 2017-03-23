using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FileContextCore.Storage;

namespace FileContextCore.ValueGeneration
{
    class FileContextIntegerValueGeneratorFactory
    {
        private FileContextCache cache;

        public FileContextIntegerValueGeneratorFactory(FileContextCache _cache)
        {
            cache = _cache;
        }

        public ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            Type t = property.ClrType;

            if(t == typeof(int))
            {
                return new FileContextIntegerValueGenerator<int>(cache.GetLastId(entityType.ClrType, property));
            }

            if(t == typeof(long))
            {
                return new FileContextIntegerValueGenerator<long>(cache.GetLastId(entityType.ClrType, property));
            }

            if (t == typeof(short))
            {
                return new FileContextIntegerValueGenerator<short>(cache.GetLastId(entityType.ClrType, property));
            }

            if (t == typeof(byte))
            {
                return new FileContextIntegerValueGenerator<byte>(cache.GetLastId(entityType.ClrType, property));
            }

            if (t == typeof(ulong))
            {
                return new FileContextIntegerValueGenerator<ulong>(cache.GetLastId(entityType.ClrType, property));
            }

            if (t == typeof(uint))
            {
                return new FileContextIntegerValueGenerator<uint>(cache.GetLastId(entityType.ClrType, property));
            }

            if (t == typeof(ushort))
            {
                return new FileContextIntegerValueGenerator<ushort>(cache.GetLastId(entityType.ClrType, property));
            }

            if (t == typeof(sbyte))
            {
                return new FileContextIntegerValueGenerator<sbyte>(cache.GetLastId(entityType.ClrType, property));
            }

            throw new ArgumentException(CoreStrings.InvalidValueGeneratorFactoryProperty(
                    nameof(FileContextIntegerValueGeneratorFactory), property.Name, property.DeclaringEntityType.DisplayName()));
        }
    }
}
