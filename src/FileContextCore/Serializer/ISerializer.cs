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

        List<T> DeserializeList<T>(string list);

        string SerializeList<T>(List<T> list);

        T Deserialize<T>(string obj);

        string Serialize<T>(T obj);
    }
}
