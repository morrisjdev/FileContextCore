using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileContextCore.StoreManager
{
    public interface IStoreManager<TKey>
    {
        void Initialize(IEntityType _entityType, IPrincipalKeyValueFactory<TKey> _keyValueFactory);
        
        Dictionary<TKey, object[]> Deserialize(Dictionary<TKey, object[]> newList);

        void Serialize(Dictionary<TKey, object[]> list);
    }
}