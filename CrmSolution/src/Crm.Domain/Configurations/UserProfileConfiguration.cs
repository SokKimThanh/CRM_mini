using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> entity)
    {
        entity.ToTable("user_profiles");
        entity.HasKey(u => u.UserId);
        entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
        entity.Property(u => u.EmployeeCode).HasMaxLength(20);
        entity.Property(u => u.RoleCode).IsRequired().HasMaxLength(20);
        entity.HasOne(u => u.User).WithOne()
            .HasForeignKey<UserProfile>(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(u => u.Team).WithMany(t => t.UserProfiles)
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}