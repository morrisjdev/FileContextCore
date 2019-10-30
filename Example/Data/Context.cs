using Microsoft.EntityFrameworkCore;
using Example.Data.Entities;
using FileContextCore;

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
            //Default: JSON-Serialize
            //optionsBuilder.UseFileContextDatabase();

            //optionsBuilder.UseFileContextDatabase("bson");

            //JSON-Serialize + simple Encryption
            //optionsBuilder.UseFileContextDatabase("json", "encrypted");

            //XML
            //optionsBuilder.UseFileContextDatabase("xml");
            //optionsBuilder.UseFileContextDatabase("xml", "private");

            //CSV
            //optionsBuilder.UseFileContextDatabase("csv", location: @"D:\t");

            //Excel
            optionsBuilder.UseFileContextDatabase("excel", databaseName: "test");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
