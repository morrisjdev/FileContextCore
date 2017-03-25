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

        public IList GetItems(Type t)
        {
            return serializer.DeserializeList(fileManager.LoadContent(t, serializer.FileType), t);
        }

        public void SaveItems(IList list)
        {
            Type t = list.GetType().GenericTypeArguments[0];

            fileManager.SaveContent(t, serializer.FileType, serializer.SerializeList(list));
        }
    }
}
