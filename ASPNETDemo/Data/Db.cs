using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ASPNETDemo.Data
{
    public class Db : DbContext
    {

        public Db(DbContextOptions<Db> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
