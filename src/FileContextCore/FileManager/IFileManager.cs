using System;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.FileManager
{
    public interface IFileManager
    {
        string GetFileName(Type t, string fileType);

        string GetFilePath(string fileName);

        string LoadContent(Type t, string fileType);

        string LoadContent(string fileName);

        void SaveContent(Type t, string fileType, string content);

        void SaveContent(string fileName, string content);

        bool Clear();

        bool DatabaseExists();
    }
}
