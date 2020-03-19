using FileContextCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileContextCore.FileManager
{
    public interface IFileManager
    {
        string GetFileName();

        string LoadContent();

        void SaveContent(string content);

        bool Clear();

        bool FileExists();
        
        void Initialize(IFileContextScopedOptions options, IEntityType entityType, string fileType);
    }
}
