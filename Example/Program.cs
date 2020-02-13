using Example.Data;
using Example.Data.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Context db = new Context();
            Console.WriteLine(db.Database.CanConnect());

            NewContext db2 = new NewContext();

            Messurement current = new Messurement();

            Stopwatch watch = new Stopwatch();

            watch.Start();



            List<User> users2 = db.Users.Include("Contents.Entries").Include("Contents").Include("Contents").ToList();

            List<User> user3 = db2.Users.ToList();
            db2.Users.RemoveRange(user3.Take(2));
            db2.SaveChanges();


            db.Users.RemoveRange(db.Users.Skip(40));
            db.SaveChanges();

            List<User> users = db.Users.Include(x => x.Contents).ThenInclude(x => x.Entries).Include(x => x.Settings).ToList(); //db.Users.Include(x => x.Contents).Include(x => x.Settings).ToList();

            List<Content> contents = db.Contents.Include(x => x.User).ToList();

            watch.Stop();
            current.TimeRead = watch.Elapsed;
            current.EntryCount = users.Count + contents.Count;
            Console.WriteLine((users.Count + contents.Count) + " Werte in " + watch.ElapsedMilliseconds + " ms gelesen");

            watch.Restart();

            User us = new User()
            {
                Name = "Morris Janatzek",
                Username = ""
            };

            db.Users.Add(us);

            User us3 = new User()
            {
                Name = "Morris Janatzek",
                Username = ""
            };

            db2.Users.Add(us3);

            db2.SaveChanges();

            db.SaveChanges();

            db.Users.Remove(db.Users.FirstOrDefault());

            db.SaveChanges();

            Content c = new Content()
            {
                Text = "Test",
                UserId = us.Id
            };

            db.Contents.Add(c);

            db.ContentEntries.Add(new ContentEntry()
            {
                Text = "das ist in set",
                ContentId = c.Id
            });

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

            watch.Stop();
            current.TimeWrite = watch.Elapsed;
            Console.WriteLine("Werte in " + watch.ElapsedMilliseconds + " ms geschrieben");

            db.Messurements.Add(current);
            db.SaveChanges();

            db.Generics.Add(new GenericTest<int>()
            {
                Value = 1
            });

            db.Generics.Add(new GenericTest<int>()
            {
                Value = 2
            });


            db.SaveChanges();

            Console.WriteLine(db.Generics.Count());

            while (true)
            {
                string result = Console.ReadLine();

                if (result != "q")
                {
                    db.Users.Add(new User()
                    {
                        Username = "testuser",
                        Name = "Test User"
                    });

                    db.SaveChanges();

                    continue;
                }

                break;
            }

            Context db3 = new Context();
            Context db4 = new Context();

            List<User> users3 = db3.Users.ToList();

            db4.Users.Add(new User()
            {
                Name = "Morris Janatzek",
                Username = "morrisj"
            });

            db4.SaveChanges();

            users3 = db3.Users.ToList();

            Console.WriteLine(db.Users.Count());

            db.Users.Add(new User()
            {
                Name = "Test",
                Username = "testname"
            });
            db.SaveChanges();

            Console.WriteLine(db.Users.Count());

            Console.ReadKey();
        }
    }
}
