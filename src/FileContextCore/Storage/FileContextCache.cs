using FileContextCore.CombinedManager;
using FileContextCore.FileManager;
using FileContextCore.Helper;
using FileContextCore.Infrastructure;
using FileContextCore.Serializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileContextCore.Storage
{
    class FileContextCache
    {
        private object thisLock = new object();

        private ICombinedManager manager;
        private Dictionary<Type, IList> cache = new Dictionary<Type, IList>();

        public FileContextCache()
        {
            manager = OptionsHelper.manager;
        }

        public IList Filter(Type t, Func<object, bool> filter)
        {
            IList result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
            IList values = (IList)GetType().GetMethod(nameof(GetValues)).MakeGenericMethod(t).Invoke(this, new object[] { });

            for(int i = 0; i < values.Count; i++)
            {
                object obj = values[i];

                if (filter.Invoke(obj))
                {
                    result.Add(obj);
                }
            }

            return result;
        }

        public List<T> GetValues<T>()
        {
            lock (thisLock)
            {
                Type t = typeof(T);

                if (cache.ContainsKey(t))
                {
                    return (List<T>)cache[t];
                }
                else
                {
                    List<T> result = manager.GetItems<T>();

                    if(result != null)
                    {
                        cache[t] = result;
                    }
                    else
                    {
                        cache[t] = new List<T>();
                    }

                    return (List<T>)cache[t];
                }
            }
        }

        public void UpdateValues<T>(List<T> newList)
        {
            lock (thisLock)
            {
                Type t = typeof(T);

                manager.SaveItems(newList);

                cache[t] = newList;
            }
        }

        public long GetLastId<T>(IProperty p)
        {
            Type t = typeof(T);
            List<T> values = GetValues<T>();

            long lastId = 0;

            if(values.Count > 0)
            {
                object obj = values[values.Count - 1];

                lastId = Convert.ToInt64(t.GetRuntimeProperty(p.Name).GetValue(obj));
            }

            return lastId++;
        }

        public void Clear()
        {
            lock (thisLock)
            {
                cache = new Dictionary<Type, IList>();
            }
        }

        public int ExecuteTransaction(IEnumerable<IUpdateEntry> entries)
        {
            lock (thisLock)
            {
                int changedCount = 0;

                IEnumerable<IGrouping<IEntityType, IUpdateEntry>> changes = entries.GroupBy(x => x.EntityType);

                foreach (IGrouping<IEntityType, IUpdateEntry> tableChange in changes)
                {
                    IEntityType tableType = tableChange.Key;
                    Type entityType = tableType.ClrType;

                    IList values = (IList)GetType().GetMethod(nameof(GetValues)).MakeGenericMethod(entityType).Invoke(this, new object[] { });

                    foreach (IUpdateEntry e in tableChange)
                    {
                        object obj = Activator.CreateInstance(entityType);

                        tableType.GetProperties().ToList().ForEach(x =>
                        {
                            entityType.GetRuntimeProperty(x.Name).SetValue(obj, e.GetCurrentValue(x));
                        });

                        if (e.EntityState == EntityState.Added)
                        {
                            values.Add(obj);
                        }
                        else if (e.EntityState == EntityState.Deleted)

                        {
                            int index = values.GetIndex(tableType, obj);

                            if (index != -1)
                            {
                                values.RemoveAt(index);
                            }
                        }
                        else if (e.EntityState == EntityState.Modified)
                        {
                            int position = values.GetIndex(tableType, obj);
                            values.RemoveAt(position);
                            values.Insert(position, obj);
                        }

                        changedCount++;
                    }

                    GetType().GetMethod(nameof(UpdateValues)).MakeGenericMethod(entityType).Invoke(this, new object[] { values });
                }

                return changedCount;
            }
        }
    }
    
}
