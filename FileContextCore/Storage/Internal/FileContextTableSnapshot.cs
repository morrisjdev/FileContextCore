// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileContextCore.Storage.Internal
{

    public class FileContextTableSnapshot
    {
    
        public FileContextTableSnapshot(
            [NotNull] IEntityType entityType,
            [NotNull] IReadOnlyList<object[]> rows)
        {
            EntityType = entityType;
            Rows = rows;
        }

    
        public virtual IEntityType EntityType { get; }

    
        public virtual IReadOnlyList<object[]> Rows { get; }
    }
}
