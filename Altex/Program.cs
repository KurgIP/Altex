using Altex.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;



namespace Altex
{
    public class Program
    {

        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //user:   Tester
            //secret: Tester_1
            //using (var scope = host.Services.CreateScope()) //That is, everytime the application fires up, It would check if the default user roles are present in the database. Else, it seeds the required Roles.
            //{
            //    var services      = scope.ServiceProvider;
            //    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            //    try
            //    {
            //        var context = services.GetRequiredService<ApplicationDbContext>();
            //        // var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            //        // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            //        // await ContextSeed.SeedRolesAsync(userManager, roleManager);
            //        // await ContextSeed.SeedSuperAdminAsync(userManager, roleManager); //Seeds SuperAdmin if it is not defined.

            //    }
            //    catch (Exception ex)
            //    {
            //        var logger = loggerFactory.CreateLogger<Program>();
            //        logger.LogError(ex, "An error occurred seeding the DB.");
            //    }
            //}
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    }
}










//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//var WebRootPath     = app.Environment.WebRootPath;
//var contentRootPath = app.Environment.ContentRootPath;


//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseMigrationsEndPoint();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();

//app.Run();
