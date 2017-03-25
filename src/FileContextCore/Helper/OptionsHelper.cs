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
        public static ICombinedManager manager = new SerializerManager(new JSONSerializer(), new DefaultFileManager());
    }
}
