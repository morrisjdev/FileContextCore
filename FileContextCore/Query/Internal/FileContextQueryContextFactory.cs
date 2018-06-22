// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FileContextCore.Infrastructure.Internal;
using FileContextCore.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;

namespace FileContextCore.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextQueryContextFactory : QueryContextFactory
    {
        private readonly IFileContextStore _store;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public FileContextQueryContextFactory(
            [NotNull] QueryContextDependencies dependencies,
            [NotNull] IFileContextStoreCache storeCache,
            [NotNull] IDbContextOptions contextOptions)
            : base(dependencies)
        {
            _store = storeCache.GetStore(contextOptions.Extensions.OfType<FileContextOptionsExtension>().First());
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override QueryContext Create()
            => new FileContextQueryContext(Dependencies, CreateQueryBuffer, _store);
    }
}
