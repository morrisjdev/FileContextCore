using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.CombinedManager
{
    public interface ICombinedManager
    {
        List<T> GetItems<T>();

        void SaveItems<T>(List<T> list);

        bool Clear();

        bool Exists();
    }
}
