using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileContextCore
{
    static class StringHelper
    {
        public static string GetValidFileName(this string input)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }

            return input;
        }
    }
}
