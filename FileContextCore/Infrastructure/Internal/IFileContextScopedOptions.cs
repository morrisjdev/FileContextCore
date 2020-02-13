using System;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.Infrastructure.Internal
{
    public interface IFileContextScopedOptions
    {
       string DatabaseName { get; }
       string Serializer { get; }
       string FileManager { get; }
       string Location { get; }
    }
}
