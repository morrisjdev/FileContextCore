using System.Collections.Generic;
using FileContextCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileContextCore.StoreManager
{
    public interface IStoreManager
    {
        void Initialize(IFileContextScopedOptions options, IEntityType entityType, object keyValueFactory);
        
        Dictionary<TKey, object[]> Deserialize<TKey>(Dictionary<TKey, object[]> newList);

        void Serialize<TKey>(Dictionary<TKey, object[]> list);
    }
}