using System;

namespace FileContextCore.Infrastructure.Internal
{
    public class FileContextScopedOptions : IFileContextScopedOptions
    {
        public FileContextScopedOptions(string databaseName, string location, string password, Type storeManagerType, Type serializerType, Type fileManagerType)
        {
            DatabaseName = databaseName;
            Location = location;
            Password = password;
            StoreManagerType = storeManagerType;
            SerializerType = serializerType;
            FileManagerType = fileManagerType;
        }

        public string DatabaseName { get; }

        public string Location { get; }
        
        public string Password { get; }

        public Type StoreManagerType { get; }
        
        public Type SerializerType { get; }
        
        public Type FileManagerType { get; }

        public override int GetHashCode()
        {
            return (DatabaseName + Location + Password + StoreManagerType + SerializerType + FileManagerType).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            FileContextScopedOptions optionsCompare = (FileContextScopedOptions) obj;
            return optionsCompare.DatabaseName == DatabaseName && optionsCompare.Location == Location &&
                   optionsCompare.Password == Password && optionsCompare.StoreManagerType == StoreManagerType && 
                   optionsCompare.SerializerType == SerializerType && optionsCompare.FileManagerType == FileManagerType;
        }
    }
}
