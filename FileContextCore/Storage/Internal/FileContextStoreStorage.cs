using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.Storage.Internal
{
    class FileContextStoreStorage
    {
        public readonly ConcurrentDictionary<string, IFileContextStore> namedStores;

        public FileContextStoreStorage()
        {
            namedStores = new ConcurrentDictionary<string, IFileContextStore>();
        }
    }
}
