using FileContextCore.CombinedManager;
using FileContextCore.FileManager;
using FileContextCore.Helper;
using FileContextCore.Infrastructure;
using FileContextCore.Serializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileContextCore.Extensions
{
    public static class FileContextDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseFileContext(this DbContextOptionsBuilder optionsBuilder, ISerializer serializer = null, IFileManager fileManager = null)
        {
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(
                new FileContextOptionsExtension());

            if(serializer != null && fileManager != null)
            {
                OptionsHelper.manager = new SerializerManager(serializer, fileManager);
            }
            else if(serializer != null)
            {
                OptionsHelper.manager = new SerializerManager(serializer, new DefaultFileManager());
            }
            else if(fileManager != null)
            {
                OptionsHelper.manager = new SerializerManager(new JSONSerializer(), fileManager);
            }

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseFileContext(this DbContextOptionsBuilder optionsBuilder, ICombinedManager manager)
        {
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(
                new FileContextOptionsExtension());

            OptionsHelper.manager = manager;

            return optionsBuilder;
        }
    }
}
