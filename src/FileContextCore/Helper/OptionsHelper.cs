using FileContextCore.FileManager;
using FileContextCore.Serializer;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.Helper
{
    static class OptionsHelper
    {
        public static IFileManager fileManager = new DefaultFileManager();

        public static ISerializer serializer = new JSONSerializer();
    }
}
