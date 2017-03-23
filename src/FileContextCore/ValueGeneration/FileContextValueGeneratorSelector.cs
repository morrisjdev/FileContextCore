using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using FileContextCore.Storage;

namespace FileContextCore.ValueGeneration
{
    class FileContextValueGeneratorSelector : ValueGeneratorSelector
    {
        private FileContextIntegerValueGeneratorFactory genFactory;

        public FileContextValueGeneratorSelector(IValueGeneratorCache cache, FileContextCache _cache) : base(cache)
        {
            genFactory = new FileContextIntegerValueGeneratorFactory(_cache);
        }

        public override ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            return
                property.ClrType == typeof(int) ||
                property.ClrType == typeof(long) ||
                property.ClrType == typeof(short) ||
                property.ClrType == typeof(byte) ||
                property.ClrType == typeof(uint) ||
                property.ClrType == typeof(ulong) ||
                property.ClrType == typeof(ushort) ||
                property.ClrType == typeof(sbyte)
                    ? genFactory.Create(property, entityType)
                    : base.Create(property, entityType);
        }
    }
}
