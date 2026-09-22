using Microsoft.AspNetCore.Identity;
using VerifyGH.Server.Models;
using VerifyGH.Shared.Enums;

namespace VerifyGH.Server.Data;

public static class DbInitializer
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        // 1. Seed Roles
        var roles = Enum.GetNames<UserRole>();
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    logger.LogInformation("Seeded role: {Role}", role);
                }
                else
                {
                    logger.LogWarning("Failed to seed role {Role}: {Errors}", role, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        // 2. Seed Default Admin User if not exists
        const string adminEmail = "admin@verifygh.edu.gh";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator",
                Institution = "VerifyGH National Verification Portal",
                Department = "ICT & Administration",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };

            var createAdminResult = await userManager.CreateAsync(adminUser, "Admin@123456");
            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, UserRole.Admin.ToString());
                logger.LogInformation("Seeded default administrator account: {Email}", adminEmail);
            }
            else
            {
                logger.LogWarning("Failed to seed admin user: {Errors}", string.Join(", ", createAdminResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
