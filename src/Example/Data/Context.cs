using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Example.Data.Entities;
using FileContextCore.Extensions;

namespace Example.Data
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Content> Contents { get; set; }

        public DbSet<Setting> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Default: JSON-Serialize
            //optionsBuilder.UseFileContext();

            //JSON-Serialize + simple Encryption
            //optionsBuilder.UseFileContext(new FileContextCore.Serializer.JSONSerializer(), new FileContextCore.FileManager.EncryptedFileManager());

            //XML
            //optionsBuilder.UseFileContext(new FileContextCore.Serializer.XMLSerializer());

            //CSV
            //optionsBuilder.UseFileContext(new FileContextCore.Serializer.CSVSerializer());

            //Excel with password
            optionsBuilder.UseFileContext(new FileContextCore.CombinedManager.ExcelManager());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
