using Crm.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Crm.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Team> Teams => Set<Team>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ===== USER PROFILE =====
        builder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(u => u.UserId);

            entity.Property(u => u.UserId).HasColumnName("user_id");
            entity.Property(u => u.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(100);
            entity.Property(u => u.EmployeeCode).HasColumnName("employee_code").HasMaxLength(20);
            entity.Property(u => u.Phone).HasColumnName("phone").HasMaxLength(20);
            entity.Property(u => u.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(500);
            entity.Property(u => u.RoleCode).HasColumnName("role_code").IsRequired().HasMaxLength(20);
            entity.Property(u => u.TeamId).HasColumnName("team_id");
            entity.Property(u => u.MonthlyTarget).HasColumnName("monthly_target").HasColumnType("numeric(18,2)");
            entity.Property(u => u.IsActive).HasColumnName("is_active");
            entity.Property(u => u.CreatedAt).HasColumnName("created_at");
            entity.Property(u => u.UpdatedAt).HasColumnName("updated_at");

            // Quan hệ 1-1: UserProfile.UserId là FK → ApplicationUser.Id (dùng navigation property)
            entity.HasOne(u => u.User)
                .WithOne()
                .HasForeignKey<UserProfile>(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với Team
            entity.HasOne(u => u.Team)
                .WithMany()
                .HasForeignKey(u => u.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ===== TEAM =====
        builder.Entity<Team>(entity =>
        {
            entity.ToTable("teams");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id).HasColumnName("id");
            entity.Property(t => t.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(t => t.ManagerId).HasColumnName("manager_id");
            entity.Property(t => t.IsActive).HasColumnName("is_active");
            entity.Property(t => t.CreatedAt).HasColumnName("created_at");

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}