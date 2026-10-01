using Crm.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crm.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, bool seedDemoData = false)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DbSeeder");

        // 1. Seed Roles
        string[] roles = { "ADMIN", "MANAGER", "SALES", "ACCOUNTANT" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                });
                logger.LogInformation("Created role: {Role}", roleName);
            }
        }

        // 2. Seed Admin
        const string adminEmail = "admin@crm.local";
        const string adminPassword = "Admin@2026";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                LockoutEnabled = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "ADMIN");
                dbContext.UserProfiles.Add(new UserProfile
                {
                    UserId = adminUser.Id,
                    FullName = "System Administrator",
                    EmployeeCode = "ADM001",
                    RoleCode = "ADMIN",
                    IsActive = true
                });
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Created admin: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Failed admin: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // 3. Seed Demo Users
        if (seedDemoData)
        {
            await SeedDemoUsersAsync(userManager, dbContext, logger);
        }
    }

    private static async Task SeedDemoUsersAsync(
        UserManager<ApplicationUser> userManager,
        AppIdentityDbContext dbContext,
        ILogger logger)
    {
        var demoUsers = new[]
        {
            new { Email = "sales1@crm.local",     Password = "Sales@2026",   Name = "Nguyễn Văn A", Code = "SAL001", Role = "SALES" },
            new { Email = "sales2@crm.local",     Password = "Sales@2026",   Name = "Trần Thị B",   Code = "SAL002", Role = "SALES" },
            new { Email = "manager@crm.local",    Password = "Manager@2026", Name = "Lê Văn C",     Code = "MGR001", Role = "MANAGER" },
            new { Email = "accountant@crm.local", Password = "Acc@2026",     Name = "Phạm Thị D",   Code = "ACC001", Role = "ACCOUNTANT" }
        };

        foreach (var u in demoUsers)
        {
            if (await userManager.FindByEmailAsync(u.Email) != null) continue;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = u.Email,
                Email = u.Email,
                EmailConfirmed = true,
                LockoutEnabled = true
            };

            var result = await userManager.CreateAsync(user, u.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, u.Role);
                dbContext.UserProfiles.Add(new UserProfile
                {
                    UserId = user.Id,
                    FullName = u.Name,
                    EmployeeCode = u.Code,
                    RoleCode = u.Role,
                    IsActive = true
                });
                logger.LogInformation("Created demo user: {Email}", u.Email);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}