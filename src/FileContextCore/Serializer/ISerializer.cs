using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileContextCore.Serializer
{
    public interface ISerializer
    {
        string FileType { get; }

        IList DeserializeList(string list, Type t);

        string SerializeList(IList list);

        object Deserialize(string obj, Type t);

        string Serialize(object obj);
    }
}
