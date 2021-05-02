using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserManagement_Backend.Models;
using Microsoft.AspNetCore.Identity;
using UserManagement_Backend.Context;
using Microsoft.Extensions.DependencyInjection;

namespace UserManagement_Backend
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                try
                {
                    // Seed Default User
                    var userManager = services.GetRequiredService<UserManager<User>>();

                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await ApplicationDbInitializer.SeedRolesAsync(userManager, roleManager);

                    await ApplicationDbInitializer.SeedDefaultUser(userManager, roleManager);

                    await ApplicationDbInitializer.SeedDefaultModeratorUser(userManager, roleManager);

                    await ApplicationDbInitializer.SeedAdministratorUser(userManager, roleManager);
                }
                catch (System.Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();

                    logger.LogError(ex, "An error occurred when seeding the database.");
                }
            }

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
