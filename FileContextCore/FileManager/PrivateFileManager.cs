using System;
using System.IO;
using FileContextCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileContextCore.FileManager
{
    public class PrivateFileManager : IFileManager
    {
        private readonly object _thisLock = new object();

        IEntityType _type;
        private string _filetype;
		private string _databasename;
        private string _location;

        public void Initialize(IFileContextScopedOptions options, IEntityType entityType, string fileType)
        {
            _type = entityType;
            _filetype = fileType;
            _databasename = options.DatabaseName ?? "";
            _location = options.Location;
        }
        
        public string GetFileName()
        {
            string name = _type.GetTableName().GetValidFileName();

            string path = string.IsNullOrEmpty(_location)
                ? Path.Combine(AppContext.BaseDirectory, "appdata", _databasename)
                : _location;

            Directory.CreateDirectory(path);

            return Path.Combine(path, name + ".private." + _filetype);
        }

        public string LoadContent()
        {
            lock (_thisLock)
            {
                string path = GetFileName();

                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }

                return "";
            }
        }

        public void SaveContent(string content)
        {
            lock (_thisLock)
            {
                string path = GetFileName();
                File.WriteAllText(path, content);

            }
        }

        public bool Clear()
        {
            lock (_thisLock)
            {
                FileInfo fi = new FileInfo(GetFileName());

                if (fi.Exists)
                {
                    fi.Delete();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool FileExists()
        {
            lock (_thisLock)
            {
                FileInfo fi = new FileInfo(GetFileName());

                return fi.Exists;
            }
        }
    }
}
