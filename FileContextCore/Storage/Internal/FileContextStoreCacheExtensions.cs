// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using FileContextCore.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FileContextCore.Storage.Internal
{

    public static class FileContextStoreCacheExtensions
    {
    
        public static IFileContextStore GetStore([NotNull] this IFileContextStoreCache storeCache, [NotNull] IDbContextOptions options)
            => storeCache.GetStore(options.Extensions.OfType<FileContextOptionsExtension>().First().Options);
    }
}
