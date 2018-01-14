using System;
using System.Collections.Generic;
using System.Globalization;
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
                return TimeSpan.Parse(input, CultureInfo.InvariantCulture);
            }
            else if(type == typeof(Guid))
            {
                return Guid.Parse(input);
            }
            else
            {
                return Convert.ChangeType(input, type, CultureInfo.InvariantCulture);
            }
        }

        public static string Serialize(this object input)
        {
            if(input != null)
            {
                IFormattable formattable = input as IFormattable;

                return formattable != null ? formattable.ToString(null, CultureInfo.InvariantCulture) : input.ToString();
            }

            return "";
        }
    }
}
