using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileContextCore.FileManager
{
    class PrivateFileManager : IFileManager
    {
        private readonly object thisLock = new object();

        IEntityType type;
        private readonly string filetype;
		private readonly string databasename;

        public PrivateFileManager(IEntityType _type, string _filetype, string _databasename)
        {
            type = _type;
            filetype = _filetype;
			databasename = _databasename;
        }

        public string GetFileName()
        {
            string name = type.Name.GetValidFileName();
			string path = Path.Combine(AppContext.BaseDirectory, "appdata", databasename);

			Directory.CreateDirectory(path);

            return Path.Combine(path, name + ".private." + filetype);
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

        private void AddEncryption()
        {
            FileInfo fi = new FileInfo(GetFileName());

            if (!fi.Exists)
            {
                fi.Create();
            }

            fi.Encrypt();
        }
    }
}
