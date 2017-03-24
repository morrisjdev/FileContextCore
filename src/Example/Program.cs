using Example.Data;
using Example.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Context db = new Context();

            //db.Database.EnsureCreated();

            //db.Database.EnsureDeleted();

            List<User> users = db.Users.Include(x => x.Contents).Include(x => x.Settings).ToList();

            List<Content> contents = db.Contents.Include(x => x.User).ToList();

            IEnumerable<User> test = db.Users;

            User us = new User()
            {
                Name = "Morris Janatzek",
                Username = "astalawixer"
            };

            db.Users.Add(us);

            db.Contents.Add(new Content()
            {
                Text = "Test",
                UserId = us.Id
            });

            db.SaveChanges();

            Console.WriteLine(db.Contents.Count());
            Console.WriteLine(db.Users.Count());

            Context db2 = new Context();
            Console.WriteLine(db2.Contents.Count());
            Console.WriteLine(db2.Users.Count());

            db.Users.Add(new User()
            {
                Username = "test123",
                Name = "Test Test"
            });

            db.Users.Add(new User()
            {
                Username = "test234",
                Name = "Test Test 2"
            });

            db.Users.Add(new User()
            {
                Username = "test567",
                Name = "Test Test 3"
            });

            db.SaveChanges();

            Console.WriteLine(db.Users.Count());
            Console.WriteLine(db2.Users.Count());

            Console.ReadKey();
        }
    }
}
