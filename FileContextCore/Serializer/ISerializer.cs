using System.Collections.Generic;
using FileContextCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileContextCore.Serializer
{
    public interface ISerializer
    {
        Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList);

        string Serialize<TKey>(Dictionary<TKey, object[]> list);

        void Initialize(IFileContextScopedOptions options, IEntityType entityType, object keyValueFactory);

        string FileType { get; }
    }
}
