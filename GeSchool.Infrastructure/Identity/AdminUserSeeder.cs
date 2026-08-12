using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GeSchool.Infrastructure.Identity;

public static class AdminUserSeeder
{
    public const string DefaultEmail = "admin@geschool.local";
    public const string DefaultPassword = "Admin@12345";

    public static async Task SeedDefaultAdminAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var usersInRole = await userManager.GetUsersInRoleAsync("Administrateur");
        if (usersInRole.Count > 0)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = DefaultEmail,
            Email = DefaultEmail,
            EmailConfirmed = true,
            Nom = "Administrateur",
            Prenom = "Système"
        };

        var result = await userManager.CreateAsync(admin, DefaultPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Administrateur");
        }
    }
}
