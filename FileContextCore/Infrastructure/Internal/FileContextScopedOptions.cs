using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FileContextCore.Infrastructure.Internal
{
    public class FileContextScopedOptions : IFileContextScopedOptions, ICloneable
    {
        public FileContextScopedOptions(string databaseName, string serializer, string fileManager, string location)
        {
            Serializer = serializer;
            DatabaseName = databaseName;
            FileManager = fileManager;
            Location = location;
        }

        public string DatabaseName { get; set; }

        public string Serializer { get; set; }

        public string FileManager { get; set; }

        public string Location { get; set; }

        public override int GetHashCode()
        {
            return (DatabaseName + Serializer + FileManager + Location).GetHashCode();
        }

        public object Clone()
        {
            return new FileContextScopedOptions(DatabaseName, Serializer, FileManager, Location);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            FileContextScopedOptions optionsCompare = (FileContextScopedOptions) obj;
            return optionsCompare.Serializer == Serializer && optionsCompare.DatabaseName == DatabaseName &&
                   optionsCompare.FileManager == FileManager && optionsCompare.Location == Location;
        }
    }
}
