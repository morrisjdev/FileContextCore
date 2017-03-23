using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileContextCore.Helper
{
    static class ArrayHelper
    {
        public static int GetIndex(this IList list, IEntityType type, object o)
        {
            List<IKey> keys = type.GetKeys().ToList();
            Type entityType = type.ClrType;

            int position = -1;

            for (int i = 0; i < list.Count; i++)
            {
                object obj = list[i];
                int testCounter = 0;

                keys.ForEach(x =>
                {
                    x.Properties.ToList().ForEach(y =>
                    {
                        object arrayValue = entityType.GetRuntimeProperty(y.Name).GetValue(obj);
                        object compareValue = entityType.GetRuntimeProperty(y.Name).GetValue(o);

                        if (y.ClrType == typeof(Guid))
                        {
                            if (((Guid)arrayValue).CompareTo((Guid)compareValue) == 0)
                            {
                                testCounter++;
                                return;
                            }
                        }

                        if (arrayValue == compareValue)
                        {
                            testCounter++;
                        }
                    });
                });

                if (testCounter == keys.Count)
                {
                    position = i;
                    break;
                }
            }

            return position;
        }
    }
}
