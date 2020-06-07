using Microsoft.EntityFrameworkCore;
using Example.Data.Entities;
using FileContextCore;
using FileContextCore.StoreManager;
using OfficeOpenXml;

namespace Example.Data
{
    public class NewContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Content> Contents { get; set; }

        public DbSet<ContentEntry> ContentEntries { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Messurement> Messurements { get; set; }

        public DbSet<GenericTest<int>> Generics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Default: JSON-Serialize
            //optionsBuilder.UseFileContextDatabase(null);
            optionsBuilder.UseFileContextDatabase(databaseName: "new");

            //JSON-Serialize + simple Encryption
            //optionsBuilder.UseFileContext("json", "encrypted");

            //XML
            //optionsBuilder.UseFileContext("xml");
            //optionsBuilder.UseFileContext("xml", "private");

            //CSV
            //optionsBuilder.UseFileContext("csv");

            //Excel
            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // optionsBuilder.UseFileContextDatabase<EXCELStoreManager>(databaseName: "excel");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
