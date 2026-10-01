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
        var dbContext   = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger      = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        foreach (var roleName in new[] { "ADMIN", "MANAGER", "SALES", "ACCOUNTANT" })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = roleName, NormalizedName = roleName });
                logger.LogInformation("Created role: {Role}", roleName);
            }
        }

        const string adminEmail = "admin@crm.local";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser { Id = Guid.NewGuid(), UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, LockoutEnabled = true };
            var r = await userManager.CreateAsync(admin, "Admin@2026");
            if (r.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "ADMIN");
                dbContext.UserProfiles.Add(new UserProfile { UserId = admin.Id, FullName = "System Administrator", EmployeeCode = "ADM001", RoleCode = "ADMIN", IsActive = true });
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Created admin: {Email}", adminEmail);
            }
        }

        if (seedDemoData)
        {
            var demos = new[]
            {
                new { Email = "sales1@crm.local",     Password = "Sales@2026",   Name = "Nguyễn Văn A", Code = "SAL001", Role = "SALES" },
                new { Email = "sales2@crm.local",     Password = "Sales@2026",   Name = "Trần Thị B",   Code = "SAL002", Role = "SALES" },
                new { Email = "manager@crm.local",    Password = "Manager@2026", Name = "Lê Văn C",     Code = "MGR001", Role = "MANAGER" },
                new { Email = "accountant@crm.local", Password = "Acc@2026",     Name = "Phạm Thị D",   Code = "ACC001", Role = "ACCOUNTANT" }
            };
            foreach (var u in demos)
            {
                if (await userManager.FindByEmailAsync(u.Email) != null) continue;
                var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = u.Email, Email = u.Email, EmailConfirmed = true, LockoutEnabled = true };
                var r = await userManager.CreateAsync(user, u.Password);
                if (r.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, u.Role);
                    dbContext.UserProfiles.Add(new UserProfile { UserId = user.Id, FullName = u.Name, EmployeeCode = u.Code, RoleCode = u.Role, IsActive = true });
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}