using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.IO;

namespace FileContextCore.FileManager
{
    class DefaultFileManager : IFileManager
    {
        private readonly object thisLock = new object();

        IEntityType type;
        private readonly string filetype;
		private readonly string databasename;

        public DefaultFileManager(IEntityType _type, string _filetype, string _databasename)
        {
            type = _type;
            filetype = _filetype;
			databasename = _databasename;
        }

        public string GetFileName()
        {
            string name = type.Name;

            foreach(char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

			string path = Path.Combine(AppContext.BaseDirectory, "appdata", databasename);
            
            Directory.CreateDirectory(path);

            return Path.Combine(path, name + "." + filetype);
        }

        public string LoadContent()
        {
            lock (thisLock)
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
            lock (thisLock)
            {
                string path = GetFileName();
                File.WriteAllText(path, content);
            }
        }

        public bool Clear()
        {
            lock (thisLock)
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
            lock (thisLock)
            {
                FileInfo fi = new FileInfo(GetFileName());

                return fi.Exists;
            }
        }
    }
}
