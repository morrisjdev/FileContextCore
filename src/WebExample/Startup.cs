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

namespace WebExample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFramework().AddDbContext<Context>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Context db = app.ApplicationServices.GetService<Context>();

            

            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/add", context =>
            {
                context.Run(async (ctx) => {
                    db.Users.Add(new Example.Data.Entities.User()
                    {
                        Username = "testuser",
                        Name = "Das ist ein test"
                    });

                    db.SaveChanges();

                    await ctx.Response.WriteAsync("added");
                });
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("User Count: " + db.Users.Count());
            });
        }
    }
}
