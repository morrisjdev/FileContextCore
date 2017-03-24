using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.CombinedManager
{
    public interface ICombinedManager
    {
        IList GetItems(Type t);

        void SaveItems(IList list);
    }
}
