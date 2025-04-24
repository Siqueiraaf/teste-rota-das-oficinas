using Microsoft.AspNetCore.Identity;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Enums;
using System.Threading.Tasks;

namespace RO.DevTest.Persistence;

public static class SeedData
{
    public static async Task SeedUsersAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        if (!await roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
        }

        if (!await roleManager.RoleExistsAsync(UserRoles.Customer.ToString()))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Customer.ToString()));
        }

        // Create admin user if it doesn't exist
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");

        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                Name = "Admin User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, UserRoles.Admin.ToString());
            }
        }

        // Create a sample customer if it doesn't exist
        var customerUser = await userManager.FindByEmailAsync("customer@example.com");

        if (customerUser == null)
        {
            customerUser = new User
            {
                UserName = "customer@example.com",
                Email = "customer@example.com",
                Name = "Customer User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(customerUser, "Customer123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(customerUser, UserRoles.Customer.ToString());
            }
        }
    }
}
