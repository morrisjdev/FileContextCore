using Example.Data;
using Example.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Context db = new Context();

            Messurement current = new Messurement();

            Stopwatch watch = new Stopwatch();

            watch.Start();

            List<User> users = db.Users.ToList(); //db.Users.Include(x => x.Contents).Include(x => x.Settings).ToList();

            List<Content> contents = db.Contents.Include(x => x.User).ToList();

            watch.Stop();
            current.TimeRead = watch.Elapsed;
            current.EntryCount = users.Count + contents.Count;
            Console.WriteLine((users.Count + contents.Count) + " Werte in " + watch.ElapsedMilliseconds + " ms gelesen");

            watch.Restart();

            User us = new User()
            {
                Name = "Morris Janatzek",
                Username = null
            };

            db.Users.Add(us);

            //db.Users.Remove(db.Users.FirstOrDefault());

            db.SaveChanges();

            db.Contents.Add(new Content()
            {
                Text = "Test",
                UserId = us.Id
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

                if(result != "q")
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
        }
    }
}
