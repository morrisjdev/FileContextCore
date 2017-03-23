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

        private ISerializer serializer;
        private IFileManager fileManager;
        private Dictionary<Type, IList> cache = new Dictionary<Type, IList>();

        public FileContextCache()
        {
            serializer = OptionsHelper.serializer;
            fileManager = OptionsHelper.fileManager;

        }

        public IList Filter(Type t, Func<object, bool> filter)
        {
            IList result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));//(IList)typeof(List<>).MakeGenericType(t).GetConstructor(new Type[] { }).Invoke(new object[] { });
            IList values = GetValues(t);

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

        public IList GetValues(Type t)
        {
            lock (thisLock)
            {
                if (cache.ContainsKey(t))
                {
                    return cache[t];
                }
                else
                {
                    IList result = serializer.DeserializeList(fileManager.LoadContent(t, serializer.FileType), t);

                    if(result != null)
                    {
                        cache[t] = result;
                    }
                    else
                    {
                        cache[t] = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t)); //(IList)typeof(List<>).MakeGenericType(t).GetConstructor(new Type[] { }).Invoke(new object[] { });
                    }

                    return cache[t];
                }
            }
        }

        public void UpdateValues(Type t, IList newList)
        {
            lock (thisLock)
            {
                fileManager.SaveContent(t, serializer.FileType, serializer.SerializeList(newList));
                cache[t] = newList;
            }
        }

        public long GetLastId(Type t, IProperty p)
        {
            IList values = GetValues(t);

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
            int changedCount = 0;

            IEnumerable<IGrouping<IEntityType, IUpdateEntry>> changes = entries.GroupBy(x => x.EntityType);

            foreach (IGrouping<IEntityType, IUpdateEntry> tableChange in changes)
            {
                IEntityType tableType = tableChange.Key;
                Type entityType = tableType.ClrType;

                IList values = GetValues(entityType);

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

                UpdateValues(entityType, values);
            }

            return changedCount;
        }
    }
}
