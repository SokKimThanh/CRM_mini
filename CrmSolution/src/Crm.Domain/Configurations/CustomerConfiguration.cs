using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> entity)
    {

        entity.HasKey(e => e.Id).HasName("customers_pkey");

        entity.HasIndex(e => e.IsActive, "idx_customers_active").HasFilter("(is_deleted = false)");

        entity.HasIndex(e => e.AssignedToUserId, "idx_customers_assigned").HasFilter("(is_deleted = false)");

        entity.HasIndex(e => e.HealthStatus, "idx_customers_health").HasFilter("(is_deleted = false)");

        entity.HasIndex(e => e.NextContactDue, "idx_customers_next_due").HasFilter("(is_deleted = false)");

        entity.Property(e => e.AverageCycleDays).HasDefaultValue(0);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        entity.Property(e => e.CurrentDebt).HasDefaultValue(0m);
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        entity.Property(e => e.OrderCount90d).HasDefaultValue(0);
        entity.Property(e => e.Revenue90d).HasDefaultValue(0m);
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

    }
}