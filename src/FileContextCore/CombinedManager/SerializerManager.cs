using FileContextCore.FileManager;
using FileContextCore.Serializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.CombinedManager
{
    public class SerializerManager : ICombinedManager
    {
        private ISerializer serializer;
        private IFileManager fileManager;

        public SerializerManager(ISerializer _serializer, IFileManager _fileManager)
        {
            serializer = _serializer;
            fileManager = _fileManager;
        }

        public bool Clear()
        {
            return fileManager.Clear();
        }

        public bool Exists()
        {
            return fileManager.DatabaseExists();
        }

        public List<T> GetItems<T>()
        {
            return serializer.DeserializeList<T>(fileManager.LoadContent<T>(serializer.FileType));
        }

        public void SaveItems<T>(List<T> list)
        {
            fileManager.SaveContent<T>(serializer.FileType, serializer.SerializeList(list));
        }
    }
}
