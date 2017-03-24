using FileContextCore.CombinedManager;
using FileContextCore.FileManager;
using FileContextCore.Serializer;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.Helper
{
    static class OptionsHelper
    {
        public static IFileManager fileManager = null;

        public static ISerializer serializer = null;

        public static ICombinedManager manager = null;
    }
}
