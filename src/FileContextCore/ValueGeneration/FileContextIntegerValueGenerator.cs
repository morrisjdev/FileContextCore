using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;

namespace FileContextCore.ValueGeneration
{
    class FileContextIntegerValueGenerator<T> : ValueGenerator<T>
    {
        public override bool GeneratesTemporaryValues => false;

        private long current;

        public FileContextIntegerValueGenerator(long _current)
        {
            current = _current;
        }

        public override T Next(EntityEntry entry)
        {
            return (T)Convert.ChangeType(Interlocked.Increment(ref current), typeof(T));
        }
    }
}
