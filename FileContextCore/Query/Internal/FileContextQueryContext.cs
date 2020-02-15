// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FileContextCore.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace FileContextCore.Query.Internal
{

    public class FileContextQueryContext : QueryContext
    {
    
        public FileContextQueryContext([NotNull] QueryContextDependencies dependencies,
            [NotNull] IFileContextStore store)
            : base(dependencies)
            => Store = store;

    
        public virtual IFileContextStore Store { get; }
    }
}
