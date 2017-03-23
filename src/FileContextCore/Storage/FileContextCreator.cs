using FileContextCore.FileManager;
using FileContextCore.Helper;
using FileContextCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileContextCore.Storage
{
    class FileContextCreator : IDatabaseCreator
    {
        private FileContextCache cache;
        private IFileManager fileManager;

        public FileContextCreator(FileContextCache _cache)
        {
            cache = _cache;
            fileManager = OptionsHelper.fileManager;
        }

        public bool EnsureCreated()
        {
            return !fileManager.DatabaseExists();
        }

        public Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(EnsureCreated());
        }

        public bool EnsureDeleted()
        {
            cache.Clear();
            return fileManager.Clear();
        }

        public Task<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(EnsureDeleted());
        }
    }
}
