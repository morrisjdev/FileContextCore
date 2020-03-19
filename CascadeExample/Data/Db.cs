using FileContextCore;
using Microsoft.EntityFrameworkCore;

namespace CascadeExample.Data
{
    public class Db : DbContext
    {
        public DbSet<User> Users { get; set; }
        
        public DbSet<Entry> Entries { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseFileContextDatabase();
        }
    }
}