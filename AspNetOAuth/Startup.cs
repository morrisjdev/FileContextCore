using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetOAuth.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FileContextCore.Extensions;

namespace AspNetOAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddDbContext<ApplicationDbContext>(options => options.UseFileContext());

			services.AddDefaultIdentity<IdentityUser>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			//services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

			services.AddAuthentication().AddGoogle(options =>
			{
				options.ClientId = "89157240943-v5mhk3sv6cdn55ko3pk7g301adlmpg3c.apps.googleusercontent.com";
				options.ClientSecret = "I8K6l613ktN_04Ovp8jrNk9W";
			}).AddFacebook(options => {
				options.AppId = "640852306260333";
				options.AppSecret = "0ca9ed875ee09715f9203a90a2f0c900";
			}).AddMicrosoftAccount(options => {
				options.ClientId = "53e67968-32ae-4080-872a-fceb3a936a21";
				options.ClientSecret = "irqwTT53:?jlfHZUFA118=#";
			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
