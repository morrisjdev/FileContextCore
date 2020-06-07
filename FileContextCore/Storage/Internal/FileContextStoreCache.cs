// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using FileContextCore.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FileContextCore.Storage.Internal
{

    public class FileContextStoreCache : IFileContextStoreCache
    {
        [NotNull] private readonly ILoggingOptions _loggingOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly bool _useNameMatching;
        private readonly ConcurrentDictionary<IFileContextScopedOptions, IFileContextStore> _namedStores;

    
        public FileContextStoreCache(
            [NotNull] ILoggingOptions loggingOptions,
            [CanBeNull] IFileContextSingletonOptions options,
            IServiceProvider serviceProvider)
        {
            _loggingOptions = loggingOptions;
            _serviceProvider = serviceProvider;
            if (options?.DatabaseRoot != null)
            {
                _useNameMatching = true;

                LazyInitializer.EnsureInitialized(
                    ref options.DatabaseRoot.Instance,
                    () => new ConcurrentDictionary<IFileContextScopedOptions, IFileContextStore>());

                _namedStores = (ConcurrentDictionary<IFileContextScopedOptions, IFileContextStore>)options.DatabaseRoot.Instance;
            }
            else
            {
                _namedStores = new ConcurrentDictionary<IFileContextScopedOptions, IFileContextStore>();
            }
        }

    
        public virtual IFileContextStore GetStore(IFileContextScopedOptions options)
        {
            return _namedStores.GetOrAdd(options, _ => new FileContextStore(new FileContextTableFactory(_loggingOptions, options, _serviceProvider), _useNameMatching));
        }
    }
}
