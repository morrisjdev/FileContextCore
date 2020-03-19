using System;
using System.Collections.Generic;
using System.Linq;
using CascadeExample.Data;
using Microsoft.EntityFrameworkCore;

namespace CascadeExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Db db = new Db();

            // List<User> users = db.Users.ToList();
            
            // db.Users.Add(new User()
            // {
            //     Name = "Test user",
            //     Entries = new List<Entry>()
            //     {
            //         new Entry() { Content = "Entry 1" },
            //         new Entry() { Content = "Entry 2" }
            //     }
            // });
            // db.SaveChanges();

            // List<User> users2 = db.Users.Include(u => u.Entries).ToList();
            //
            db.Users.RemoveRange(db.Users.Include(u => u.Entries));
            db.SaveChanges();
        }
    }
}