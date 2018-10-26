// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FileContextCore.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Concurrent;

namespace FileContextCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextStoreCache : IFileContextStoreCache
    {
        private readonly IFileContextTableFactory _tableFactory;
        private readonly FileContextStoreStorage _fileContextStoreStorage;

		//private IFileContextStore _store;

		/// <summary>
		///     This API supports the Entity Framework Core infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public FileContextStoreCache([NotNull] IFileContextTableFactory tableFactory, FileContextStoreStorage fileContextStoreStorage)
        {
            _tableFactory = tableFactory;
            _fileContextStoreStorage = fileContextStoreStorage;
		}

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IFileContextStore GetStore(FileContextOptionsExtension options)
        {
			//if (_store == null)
			//{
			//	_store = new FileContextStore(_tableFactory);
			//}

			//return _store;
			return _fileContextStoreStorage.namedStores.GetOrAdd(options.DatabaseName, n => new FileContextStore(_tableFactory, options));
		}
    }
}
