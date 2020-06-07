using System;
using System.Collections.Generic;
using FileContextCore.FileManager;
using FileContextCore.Infrastructure.Internal;
using FileContextCore.Serializer;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileContextCore.StoreManager
{
    class DefaultStoreManager : IStoreManager {
        private readonly IServiceProvider _serviceProvider;
        private ISerializer _serializer;
        private IFileManager _fileManager;
        private object _keyValueFactory;
        private IEntityType _entityType;

        public DefaultStoreManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public void Initialize(IFileContextScopedOptions options, IEntityType entityType,
            object keyValueFactory)
        {
            _keyValueFactory = keyValueFactory;
            _entityType = entityType;

            _serializer = (ISerializer)_serviceProvider.GetService(options.SerializerType);
            _serializer.Initialize(options, _entityType, _keyValueFactory);

            _fileManager = (IFileManager)_serviceProvider.GetService(options.FileManagerType);
            _fileManager.Initialize(options, entityType, _serializer.FileType);
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(Dictionary<TKey, object[]> newList)
        {
            string content = _fileManager.LoadContent();
            return _serializer.Deserialize(content, newList);
        }

        public void Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            string cnt = _serializer.Serialize(list);
            _fileManager.SaveContent(cnt);
        }
    }
}