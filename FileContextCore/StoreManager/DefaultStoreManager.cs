using System.Collections.Generic;
using CsvHelper;
using FileContextCore.FileManager;
using FileContextCore.Serializer;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileContextCore.StoreManager
{
    public class DefaultStoreManager<TKey, TSerializer, TFileManager> : IStoreManager<TKey>
        where TSerializer : ISerializer<TKey>
        where TFileManager : IFileManager
    {
        public void Initialize(IEntityType _entityType, IPrincipalKeyValueFactory<TKey> _keyValueFactory)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<TKey, object[]> Deserialize(Dictionary<TKey, object[]> newList)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(Dictionary<TKey, object[]> list)
        {
            throw new System.NotImplementedException();
        }
    }
}