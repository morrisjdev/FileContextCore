using System;

namespace FileContextCore.Infrastructure.Internal
{
    public interface IFileContextScopedOptions
    {
       string DatabaseName { get; }
       string Location { get; }
       string Password { get; }
       Type StoreManagerType { get; }
       Type SerializerType { get; }
       Type FileManagerType { get; }
    }
}
