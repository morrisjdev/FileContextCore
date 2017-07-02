using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileContextCore.Helper
{
    static class PropertyHelper
    {
        public static PropertyInfo[] GetPropertiesForSerialize(this Type t)
        {
            return t.GetRuntimeProperties().Where(x =>
                (!x.GetMethod.IsVirtual || x.GetMethod.IsFinal) &&
                !x.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute)))
                .ToArray();
        }

        public static PropertyInfo[] GetPropertiesNotForSerialize(this Type t)
        {
            return t.GetRuntimeProperties().Where(x =>
                (x.GetMethod.IsVirtual && !x.GetMethod.IsFinal) ||
                x.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute)))
                .ToArray();
        }

        public static bool ShouldSerialize(this PropertyInfo prop)
        {
            return (!prop.GetMethod.IsVirtual || prop.GetMethod.IsFinal) &&
                !prop.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute));
        }
    }
}
