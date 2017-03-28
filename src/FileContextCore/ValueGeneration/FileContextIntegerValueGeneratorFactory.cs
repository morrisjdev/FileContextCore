using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FileContextCore.Storage;
using FileContextCore.Helper;
using System.Reflection;

namespace FileContextCore.ValueGeneration
{
    class FileContextIntegerValueGeneratorFactory
    {
        public FileContextIntegerValueGeneratorFactory()
        {
        }

        public ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            Type t = property.ClrType;
            long lastId = (long)QueryHelper.cache.GetType().GetMethod(nameof(QueryHelper.cache.GetLastId)).MakeGenericMethod(entityType.ClrType).Invoke(QueryHelper.cache, new object[] { property });

            if (t == typeof(int))
            {
                return new FileContextIntegerValueGenerator<int>(lastId);
            }

            if(t == typeof(long))
            {
                return new FileContextIntegerValueGenerator<long>(lastId);
            }

            if (t == typeof(short))
            {
                return new FileContextIntegerValueGenerator<short>(lastId);
            }

            if (t == typeof(byte))
            {
                return new FileContextIntegerValueGenerator<byte>(lastId);
            }

            if (t == typeof(ulong))
            {
                return new FileContextIntegerValueGenerator<ulong>(lastId);
            }

            if (t == typeof(uint))
            {
                return new FileContextIntegerValueGenerator<uint>(lastId);
            }

            if (t == typeof(ushort))
            {
                return new FileContextIntegerValueGenerator<ushort>(lastId);
            }

            if (t == typeof(sbyte))
            {
                return new FileContextIntegerValueGenerator<sbyte>(lastId);
            }

            throw new ArgumentException(CoreStrings.InvalidValueGeneratorFactoryProperty(
                    nameof(FileContextIntegerValueGeneratorFactory), property.Name, property.DeclaringEntityType.DisplayName()));
        }
    }
}
