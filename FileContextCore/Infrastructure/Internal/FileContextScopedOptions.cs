using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FileContextCore.Infrastructure.Internal
{
    public class FileContextScopedOptions : IFileContextScopedOptions
    {
        public FileContextScopedOptions(string databaseName, string location, string password, Type storeManagerType)
        {
            DatabaseName = databaseName;
            Location = location;
            Password = password;
            StoreManagerType = storeManagerType;
        }

        public string DatabaseName { get; }

        public string Location { get; }
        
        public string Password { get; }

        public Type StoreManagerType { get; }

        public override int GetHashCode()
        {
            return (DatabaseName + Location + Password + StoreManagerType).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            FileContextScopedOptions optionsCompare = (FileContextScopedOptions) obj;
            return optionsCompare.DatabaseName == DatabaseName && optionsCompare.Location == Location &&
                   optionsCompare.Password == Password && optionsCompare.StoreManagerType == StoreManagerType;
        }
    }
}
