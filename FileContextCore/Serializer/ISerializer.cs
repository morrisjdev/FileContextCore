using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileContextCore.Serializer
{
    public interface ISerializer<TKey>
    {
        Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList);

        string Serialize<TKey>(Dictionary<TKey, object[]> list);

        void Initialize(IEntityType _entityType, IPrincipalKeyValueFactory<TKey> _keyValueFactory);

        // string FileType { get; }
    }
}
