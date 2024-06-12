using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
using Altex.Data;
//using Community.Microsoft.Extensions.Caching.PostgreSql;

namespace Altex
{
    public class Startup
    {
        public static string  _serverRootPath { get; private set; } = "";

        public IConfiguration Configuration { get; }

        //public static IConfiguration StaticConfig { get; private set; }
        
        //public static IHttpContextAccessor _httpContextAccessor { get; private set; }
        //public static UserManager<ApplicationUser> _StaticUserManager { get; private set; }

        public Startup(IHostingEnvironment env, IConfiguration configuration ) //
        {
            Configuration   = configuration;
            //StaticConfig    = configuration;
            _serverRootPath = env.ContentRootPath;
        }

        //public static ApplicationUser              _app_user            { get; private set; }

        //public static ILoggerManager _logger { get; private set; }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string postgreSq_lConnection = Configuration.GetConnectionString("PostgreSqlConnection");
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(postgreSq_lConnection) 
                );
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddControllersWithViews().AddRazorRuntimeCompilation(); //This is added for to see cshtml changes without restarting app.
            services.AddControllersWithViews();

            services.AddMvc();

            //services.AddDistributedPostgreSqlCache(setup =>
            //{
            //    setup.ConnectionString = Configuration.GetSection("Sessions").GetValue<string>("ConnectionString");
            //    setup.SchemaName       = Configuration.GetSection("Sessions").GetValue<string>("SchemaName");
            //    setup.TableName        = Configuration.GetSection("Sessions").GetValue<string>("TableName");

            //    // Optional - DisableRemoveExpired default is FALSE                
            //    setup.DisableRemoveExpired = Configuration.GetSection("Sessions").GetValue<bool>("DisableRemoveExpired");

            //    // CreateInfrastructure is optional, default is TRUE This means que every time starts the application the  creation of table and database functions will be verified.
            //    setup.CreateInfrastructure = Configuration.GetSection("Sessions").GetValue<bool>("CreateInfrastructure");

            //    // ExpiredItemsDeletionInterval is optional
            //    // This is the periodic interval to scan and delete expired items in the cache. Default is 30 minutes.
            //    // Minimum allowed is 5 minutes. - If you need less than this please share your use case 😁, just for curiosity...
            //    setup.ExpiredItemsDeletionInterval = Configuration.GetSection("Sessions").GetValue<TimeSpan>("ExpiredItemsDeletionInterval");  // TimeSpan.FromMinutes(30)
            //});

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IHttpContextAccessor httpContextAccessor) //, UserManager<ApplicationUser> manager
        {
            //_httpContextAccessor = httpContextAccessor;
            _serverRootPath = env.ContentRootPath;
            //_StaticUserManager = manager;

            // setup app's root folders
            //AppDomain.CurrentDomain.SetData("ContentRootPath", env.ContentRootPath);
            //AppDomain.CurrentDomain.SetData("WebRootPath", env.EnvironmentName);

            //_app_user            = applicationUser; , ApplicationUser applicationUser 
            //var user = await manager.GetUserAsync.GetUserAsync( Startup._httpContextAccessor.HttpContext.User );

            //if (env.IsDevelopment())HostEnviropmentEnvExtension
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //if (env.IsDevelopment())
            if (env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            app.UseStaticFiles();
            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

        }

    }
}
