using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Example.Data;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebExample.Controllers
{
    public class HomeController : Controller
    {
        private Context db;

        public HomeController(Context _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListAge(int age)
        {
            return Json(db.Users.Where(x => x.Id > age));
        }

        public IActionResult List()
        {
            return View(db.Users.Include(x => x.Contents));
        }

        public string Add()
        {
            for (int i = 0; i < 20; i++)
            {
                var newUser = new Example.Data.Entities.User()
                {
                    Username = "testuser",
                    Name = "Das ist ein test"
                };

                db.Users.Add(newUser);

                db.Contents.Add(new Example.Data.Entities.Content()
                {
                    UserId = newUser.Id,
                    Text = "das ist ein test"
                });
            }

            db.SaveChanges();

            return "added";
        }
    }
}
