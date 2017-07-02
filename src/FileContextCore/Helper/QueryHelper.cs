using FileContextCore.Query;
using FileContextCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FileContextCore.Helper
{
    static class QueryHelper
    {
        public static FileContextCache cache;

        public static IEnumerable<T> GetValues<T>()
        {
            return cache.GetValues<T>();
        }

        private static IList LoadData(IList values, IEnumerable<INavigation> navs)
        {
            INavigation nav = navs.FirstOrDefault();

            if (nav != null)
            {
                IEntityType t;
                IClrPropertyGetter[] objProps;
                IClrPropertyGetter[] refObjProps;

                if (nav.IsDependentToPrincipal())
                {
                    t = nav.ForeignKey.PrincipalEntityType;

                    objProps = nav.ForeignKey.Properties.Select(x => x.GetGetter()).ToArray();
                    refObjProps = nav.ForeignKey.PrincipalKey.Properties.Select(x => x.GetGetter()).ToArray();
                }
                else
                {
                    t = nav.ForeignKey.DeclaringEntityType;

                    objProps = nav.ForeignKey.PrincipalKey.Properties.Select(x => x.GetGetter()).ToArray();
                    refObjProps = nav.ForeignKey.Properties.Select(x => x.GetGetter()).ToArray();
                }

                if (objProps.Count() != refObjProps.Count())
                {
                    return values;
                }

                IClrPropertySetter valueSetter = nav.GetSetter();

                if (nav.IsCollection())
                {
                    foreach (object obj in values)
                    {
                        object[] objValues = objProps.Select(x => x.GetClrValue(obj)).ToArray();

                        IList rValues = cache.Filter(t.ClrType, x =>
                        {
                            int result = 0;

                            object[] refValues = refObjProps.Select(y => y.GetClrValue(x)).ToArray();

                            if (refValues.Count() == objValues.Count())
                            {
                                for (int i = 0; i < refValues.Count(); i++)
                                {
                                    if (Equals(objValues[i], refValues[i]))
                                    {
                                        result++;
                                    }
                                }
                            }

                            return result == refValues.Count();
                        });

                        if(navs.Count() > 1)
                        {
                            rValues = LoadData(rValues, navs.Skip(1));
                        }

                        valueSetter.SetClrValue(obj, rValues);
                    }
                }
                else
                {
                    foreach (object obj in values)
                    {
                        object[] objValues = objProps.Select(x => x.GetClrValue(obj)).ToArray();

                        IList matching = cache.Filter(t.ClrType, x =>
                        {
                            int result = 0;

                            object[] refValues = refObjProps.Select(y => y.GetClrValue(x)).ToArray();

                            if (refValues.Count() == objValues.Count())
                            {
                                for (int i = 0; i < refValues.Count(); i++)
                                {
                                    if (Equals(objValues[i], refValues[i]))
                                    {
                                        result++;
                                    }
                                }
                            }

                            return result == refValues.Count();
                        });

                        if (matching.Count > 0)
                        {
                            object value = matching[0];

                            if(navs.Count() > 1)
                            {
                                value = LoadData(new object[] { value }, navs.Skip(1))[0];
                            }

                            valueSetter.SetClrValue(obj, value);
                        }
                    }
                }
            }

            return values;
        }

        public static IEnumerable<T> LoadRelatedData<T>(IEnumerable<T> values, IncludeSpecification includeSpecification)
        {
            return LoadData(values.ToList(), includeSpecification.NavigationPath).Cast<T>();
        }
    }
}
