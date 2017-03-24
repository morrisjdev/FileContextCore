using FileContextCore.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileContextCore.FileManager
{
    public class DefaultFileManager : IFileManager
    {
        private object thisLock = new object();

        public string GetFileName(Type t, string fileType)
        {
            return GetFilePath(t.Name + "." + fileType);
        }

        public string GetFilePath(string fileName)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "appdata");
            Directory.CreateDirectory(path);

            return Path.Combine(path, fileName);
        }

        public string LoadContent(Type t, string fileType)
        {
            return LoadContent(GetFileName(t, fileType));
        }

        public string LoadContent(string fileName)
        {
            lock (thisLock)
            {
                string path = GetFilePath(fileName);

                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }

                return "";
            }
        }

        public void SaveContent(Type t, string fileType, string content)
        {
            SaveContent(GetFileName(t, fileType), content);
        }

        public void SaveContent(string fileName, string content)
        {
            lock (thisLock)
            {
                string path = GetFilePath(fileName);

                File.WriteAllText(path, content);
            }
        }

        public bool Clear()
        {
            lock (thisLock)
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "appdata"));

                if (di.Exists)
                {
                    di.Delete(true);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool DatabaseExists()
        {
            lock (thisLock)
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "appdata"));
                return di.Exists;
            }
        }
    }
}
