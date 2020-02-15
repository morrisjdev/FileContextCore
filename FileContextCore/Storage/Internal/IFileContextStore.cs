// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using FileContextCore.ValueGeneration.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;

namespace FileContextCore.Storage.Internal
{

    public interface IFileContextStore
    {
    
        bool EnsureCreated(
            [NotNull] IUpdateAdapterFactory updateAdapterFactory,
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger);

    
        bool Clear();

    
        IReadOnlyList<FileContextTableSnapshot> GetTables([NotNull] IEntityType entityType);

    
        FileContextIntegerValueGenerator<TProperty> GetIntegerValueGenerator<TProperty>([NotNull] IProperty property);

    
        int ExecuteTransaction(
            [NotNull] IList<IUpdateEntry> entries, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger);
    }
}
