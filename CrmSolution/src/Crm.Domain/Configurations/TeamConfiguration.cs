using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> entity)
    {
        entity.ToTable("teams");
        entity.HasKey(t => t.Id);
        entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
        entity.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(t => t.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}