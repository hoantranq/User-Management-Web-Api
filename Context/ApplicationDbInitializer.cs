using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using UserManagement_Backend.Helpers;
using UserManagement_Backend.Models;

namespace UserManagement_Backend.Context
{
    public class ApplicationDbInitializer
    {
        public static async Task SeedRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.Administrator.ToString().ToLower()));
            await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.Moderator.ToString().ToLower()));
            await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.User.ToString().ToLower()));
        }

        public static async Task SeedDefaultUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Default Normal User
            var defaultUser = new User
            {
                UserName = Authorization.DEFAULT_USERNAME,
                Email = Authorization.DEFAULT_EMAIL,
                EmailConfirmed = true,
            };

            if (userManager.Users.All(user => user.Id != defaultUser.Id))
            {
                await userManager.CreateAsync(defaultUser, Authorization.DEFAULT_PASSWORD);
                await userManager.AddToRoleAsync(defaultUser, Authorization.DEFAULT_ROLE.ToString());
            }
        }

        public static async Task SeedDefaultModeratorUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Default Normal User
            var defaultUser = new User
            {
                UserName = "moderatorUser",
                Email = "moderatoruser@gmail.com",
                EmailConfirmed = true,
            };

            if (userManager.Users.All(user => user.Id != defaultUser.Id))
            {
                await userManager.CreateAsync(defaultUser, Authorization.DEFAULT_PASSWORD);
                await userManager.AddToRoleAsync(defaultUser, Authorization.DEFAULT_ROLE.ToString());
            }
        }

        public static async Task SeedAdministratorUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Default Normal User
            var defaultUser = new User
            {
                UserName = "administratoruser",
                Email = "administratoruser@gmail.com",
                EmailConfirmed = true,
            };

            if (userManager.Users.All(user => user.Id != defaultUser.Id))
            {
                await userManager.CreateAsync(defaultUser, Authorization.DEFAULT_PASSWORD);
                await userManager.AddToRoleAsync(defaultUser, Authorization.DEFAULT_ROLE.ToString());
            }
        }
    }
}
