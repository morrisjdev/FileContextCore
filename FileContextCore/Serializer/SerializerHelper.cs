using System;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.Serializer
{
    static class SerializerHelper
    {
        public static object Deserialize(this string input, Type type)
        {
            if (String.IsNullOrEmpty(input))
            {
                return type.GetDefaultValue();
            }

            if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(input);
            }
            else if(type == typeof(Guid))
            {
                return Guid.Parse(input);
            }
            else
            {
                return Convert.ChangeType(input, type);
            }
        }

        public static string Serialize(this object input)
        {
            if(input != null)
            {
                return input.ToString();
            }

            return "";
        }
    }
}
