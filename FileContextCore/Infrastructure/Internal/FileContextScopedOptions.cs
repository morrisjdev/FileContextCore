using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FileContextCore.Infrastructure.Internal
{
    public class FileContextScopedOptions : IFileContextScopedOptions
    {
        public FileContextScopedOptions(FileContextOptionsExtension options)
        {
            if (options != null)
            {
                Serializer = options.serializer;
                DatabaseName = options.StoreName;
                FileManager = options.filemanager;
                Location = options.location;
            }
        }

        public virtual string DatabaseName { get; private set; }

        public virtual string Serializer { get; private set; }

        public virtual string FileManager { get; private set; }

        public virtual string Location { get; private set; }
    }
}
