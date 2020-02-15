// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using FileContextCore.ValueGeneration.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;

namespace FileContextCore.Storage.Internal
{

    public interface IFileContextTable
    {
    
        IReadOnlyList<object[]> SnapshotRows();

    
        void Create([NotNull] IUpdateEntry entry);

    
        void Delete([NotNull] IUpdateEntry entry);

    
        void Update([NotNull] IUpdateEntry entry);

    
        FileContextIntegerValueGenerator<TProperty> GetIntegerValueGenerator<TProperty>([NotNull] IProperty property);

        void Save();
    }
}
