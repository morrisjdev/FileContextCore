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
        public static DbContextOptionsBuilder UseFileContext(this DbContextOptionsBuilder optionsBuilder)
        {
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(
                new FileContextOptionsExtension());

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseFileContext(this DbContextOptionsBuilder optionsBuilder, ISerializer serializer = null, IFileManager fileManager = null)
        {
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(
                new FileContextOptionsExtension());

            if(serializer != null)
            {
                OptionsHelper.serializer = serializer;
            }
            else
            {
                OptionsHelper.serializer = new JSONSerializer();
            }

            if (fileManager != null)
            {
                OptionsHelper.fileManager = fileManager;
            }
            else
            {
                OptionsHelper.fileManager = new DefaultFileManager();
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
