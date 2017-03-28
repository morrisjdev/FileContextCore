using System;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.FileManager
{
    public interface IFileManager
    {
        string GetFileName<T>(string fileType);

        string GetFilePath(string fileName);

        string LoadContent<T>(string fileType);

        string LoadContent(string fileName);

        void SaveContent<T>(string fileType, string content);

        void SaveContent(string fileName, string content);

        bool Clear();

        bool DatabaseExists();
    }
}
