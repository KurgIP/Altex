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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
//using Community.Microsoft.Extensions.Caching.PostgreSql;

namespace Altex
{
    public class Startup
    {
        public static  string                _serverRootPath       { get; private set; } = "";
        public static  ILogger<Controller>   _logerStatic          { get; private set; }
        public static  IConfiguration        _configurationStatic  { get; private set; }
        public static  IHttpContextAccessor  _httpContextAccessor  { get; private set; }
        public         IConfiguration        Configuration         { get; }
        public static UserManager<IdentityUser> _StaticUserManager { get; private set; }

        //public static IConfiguration StaticConfig { get; private set; }        
        //public static ApplicationUser     _app_user            { get; private set; }
        //public static ILoggerManager _logger { get; private set; }
        // public Startup(IHostingEnvironment env, IConfiguration configuration, HttpContext context, ILogger<Controller> logger ) //, IHttpContextAccessor contextAccessor, ILogger<Controller> logger
        
        public Startup(IHostingEnvironment env, IConfiguration configuration) //
        {
            Configuration        = configuration;
            _serverRootPath      = env.ContentRootPath;
            _configurationStatic = configuration;
            //_httpContextAccessor = contextAccessor;
            //_httpContextStatic   = context;
            //_logerStatic         = logger;
        }



        // This method gets called by the runtime. Use this method to add services to the container. IWebHostBuilder
        public void ConfigureServices(IServiceCollection services)
        {
            string postgreSq_lConnection = Configuration.GetConnectionString("PostgreSqlConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(postgreSq_lConnection)
                );
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                        .AddRoles<IdentityRole>()
                        .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit           = true;
                options.Password.RequireLowercase       = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase       = true;
                options.Password.RequiredLength         = 6;
                options.Password.RequiredUniqueChars    = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers      = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan  = TimeSpan.FromMinutes(5);

                options.LoginPath         = "/Identity/Account/Login";
                options.AccessDeniedPath  = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            //services.AddIdentity<ApplicationUser, IdentityRole>(
            //    //options => { options.SignIn.RequireConfirmedAccount = true; }
            //    )
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            //    //.AddDefaultTokenProviders();


            //services.AddControllersWithViews().AddRazorRuntimeCompilation(); //This is added for to see cshtml changes without restarting app.


            services.AddMvc(MvcOptions => MvcOptions.EnableEndpointRouting = false);

            ////// аутентификация с помощью куки
            ////services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            ////    .AddCookie(options => options.LoginPath = "/Account/Login");

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //                            .AddCookie(options =>
            //                            {
            //                                options.LoginPath         = "/Account/Login";
            //                                options.ExpireTimeSpan    = TimeSpan.FromMinutes(20);
            //                                options.SlidingExpiration = true;
            //                                options.AccessDeniedPath  = "/Home/Error";
            //                            });
            //services.AddAuthorization();



            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddSession(options =>
            {
                options.IdleTimeout        = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly    = true;
                options.Cookie.IsEssential = true;
            });

            //services.AddRazorPages();



            //string postgreSq_lConnection = Configuration.GetConnectionString("PostgreSqlConnection");
            ////services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddHttpContextAccessor();
            //services.AddSingleton<IConfiguration>(Configuration);
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseNpgsql(postgreSq_lConnection) 
            //    );

            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            //// аутентификация с помощью куки
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options => options.LoginPath = "/Account/Login");
            //services.AddAuthorization();

            //services.AddLogging(config =>
            //{
            //    config.AddDebug();
            //    config.AddConsole();
            //    config.SetMinimumLevel( LogLevel.Information );
            //});

            //services.AddDatabaseDeveloperPageExceptionFilter();
            //services.AddControllersWithViews().AddRazorRuntimeCompilation(); //This is added for to see cshtml changes without restarting app.
            //services.AddControllersWithViews();
            //services.AddMvc();

            ////services.AddDistributedPostgreSqlCache(setup =>
            ////{
            ////    setup.ConnectionString = Configuration.GetSection("Sessions").GetValue<string>("ConnectionString");
            ////    setup.SchemaName       = Configuration.GetSection("Sessions").GetValue<string>("SchemaName");
            ////    setup.TableName        = Configuration.GetSection("Sessions").GetValue<string>("TableName");

            ////    // Optional - DisableRemoveExpired default is FALSE                
            ////    setup.DisableRemoveExpired = Configuration.GetSection("Sessions").GetValue<bool>("DisableRemoveExpired");

            ////    // CreateInfrastructure is optional, default is TRUE This means que every time starts the application the  creation of table and database functions will be verified.
            ////    setup.CreateInfrastructure = Configuration.GetSection("Sessions").GetValue<bool>("CreateInfrastructure");

            ////    // ExpiredItemsDeletionInterval is optional
            ////    // This is the periodic interval to scan and delete expired items in the cache. Default is 30 minutes.
            ////    // Minimum allowed is 5 minutes. - If you need less than this please share your use case 😁, just for curiosity...
            ////    setup.ExpiredItemsDeletionInterval = Configuration.GetSection("Sessions").GetValue<TimeSpan>("ExpiredItemsDeletionInterval");  // TimeSpan.FromMinutes(30)
            ////});

            //services.AddSession(options =>
            //{
            //    options.IdleTimeout        = TimeSpan.FromMinutes(30);
            //    options.Cookie.HttpOnly    = true;
            //    options.Cookie.IsEssential = true;
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline., 
        public void Configure( IApplicationBuilder app, IHostingEnvironment env, UserManager<IdentityUser> manager, ILogger<Controller> logger, IHttpContextAccessor contextAccessor ) //
        {
            _httpContextAccessor = contextAccessor;
            _serverRootPath      = env.ContentRootPath;
            _logerStatic         = logger;
            _StaticUserManager   = manager;


            //_httpContextStatic = context;
            //_httpContextStatic = context;
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

            if (env.IsDevelopment())
            {
               
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //    //endpoints.MapRazorPages();
            //});

        }

    }
}
