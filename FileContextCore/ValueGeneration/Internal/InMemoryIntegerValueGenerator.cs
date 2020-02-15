// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace FileContextCore.ValueGeneration.Internal
{

    public class FileContextIntegerValueGenerator<TValue> : ValueGenerator<TValue>, IFileContextIntegerValueGenerator
    {
        private readonly int _propertyIndex;
        private long _current;

    
        public FileContextIntegerValueGenerator(int propertyIndex) => _propertyIndex = propertyIndex;

    
        public virtual void Bump(object[] row)
        {
            var newValue = (long)Convert.ChangeType(row[_propertyIndex], typeof(long));

            if (_current < newValue)
            {
                Interlocked.Exchange(ref _current, newValue);
            }
        }

    
        public override TValue Next(EntityEntry entry)
            => (TValue)Convert.ChangeType(Interlocked.Increment(ref _current), typeof(TValue), CultureInfo.InvariantCulture);

    
        public override bool GeneratesTemporaryValues => false;
    }
}
