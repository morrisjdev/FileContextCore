// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using System.Threading;
using FileContextCore.Infrastructure.Internal;
using JetBrains.Annotations;

namespace FileContextCore.Storage.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class FileContextStoreCache : IFileContextStoreCache
    {
        private readonly IFileContextTableFactory _tableFactory;
        private readonly bool _useNameMatching;
        private readonly ConcurrentDictionary<string, IFileContextStore> _namedStores;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public FileContextStoreCache(
            [NotNull] IFileContextTableFactory tableFactory,
            [CanBeNull] IFileContextSingletonOptions options)
        {
            _tableFactory = tableFactory;

            if (options?.DatabaseRoot != null)
            {
                _useNameMatching = true;

                LazyInitializer.EnsureInitialized(
                    ref options.DatabaseRoot.Instance,
                    () => new ConcurrentDictionary<string, IFileContextStore>());

                _namedStores = (ConcurrentDictionary<string, IFileContextStore>)options.DatabaseRoot.Instance;
            }
            else
            {
                _namedStores = new ConcurrentDictionary<string, IFileContextStore>();
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IFileContextStore GetStore(string name)
            => _namedStores.GetOrAdd(name, _ => new FileContextStore(_tableFactory, _useNameMatching));
    }
}
