using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Example.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace WebExample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddEntityFramework().AddDbContext<Context>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Context db = app.ApplicationServices.GetService<Context>();

            

            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /*app.Map("/add", context =>
            {
                context.Run(async (ctx) => {

                    for(int i = 0; i < 20; i++)
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

                    await ctx.Response.WriteAsync("added");
                });
            });

            app.Map("/list", context =>
            {

                context.Run(async (ctx) =>
                {
                    string response = "";

                    db.Users.Include(x => x.Contents).ToList().ForEach(u =>
                    {
                        response += u.Username + " ---- ";

                        u.Contents.ForEach(x =>
                        {
                            response += x.Text;
                        });

                        response += "<br>";
                    });

                    ctx.Response.ContentType = "text/html";
                    await ctx.Response.WriteAsync(response);
                });
            });*/

            app.UseMvcWithDefaultRoute();

            /*app.Run(async (context) =>
            {
                await context.Response.WriteAsync("User Count: " + db.Users.Count());
            });*/
        }
    }
}
