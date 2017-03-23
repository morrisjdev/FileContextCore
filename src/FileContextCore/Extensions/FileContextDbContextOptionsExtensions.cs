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

            if (fileManager != null)
            {
                OptionsHelper.fileManager = fileManager;
            }

            return optionsBuilder;
        }
    }
}
