using Example.Data.Entities;
using FileContextCore;
using FileContextCore.StoreManager;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Example.Data
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Content> Contents { get; set; }

        public DbSet<ContentEntry> ContentEntries { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Messurement> Messurements { get; set; }

        public DbSet<GenericTest<int>> Generics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Default: JSON-Serializer
            optionsBuilder.UseFileContextDatabase();

            // optionsBuilder.UseFileContextDatabase<JSONSerializer, DefaultFileManager>();

            // optionsBuilder.UseFileContextDatabase<BSONSerializer, DefaultFileManager>();

            //JSON-Serialize + simple Encryption
            // optionsBuilder.UseFileContextDatabase<JSONSerializer, EncryptedFileManager>();

            //XML
            // optionsBuilder.UseFileContextDatabase<XMLSerializer, DefaultFileManager>();
            // optionsBuilder.UseFileContextDatabase<XMLSerializer, PrivateFileManager>();

            //CSV
            // optionsBuilder.UseFileContextDatabase<CSVSerializer, DefaultFileManager>();

            //Custom location
            // optionsBuilder.UseFileContextDatabase(location: @"D:\t");

            //Excel
            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // optionsBuilder.UseFileContextDatabase<EXCELStoreManager>(databaseName: "test");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("custom_user_table");
        }
    }
}
