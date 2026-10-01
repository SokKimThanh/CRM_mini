using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class SalesTaskConfiguration : IEntityTypeConfiguration<SalesTask>
{
    public void Configure(EntityTypeBuilder<SalesTask> entity)
    {

            entity.HasKey(e => e.Id).HasName("sales_tasks_pkey");

            entity.HasIndex(e => e.AssignedToUserId, "idx_tasks_assigned").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.CustomerId, "idx_tasks_customer").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.DueDate, "idx_tasks_due").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.Status, "idx_tasks_status").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => new { e.AssignedToUserId, e.DueDate, e.Status }, "idx_tasks_today").HasFilter("((status = ANY (ARRAY[0, 1])) AND (is_deleted = false))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Priority).HasDefaultValue(1);
            entity.Property(e => e.Type).HasDefaultValue(6);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Contact).WithMany(p => p.SalesTasks)
                .OnDelete(DeleteBehavior.SetNull)
                ;

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesTasks)
                .OnDelete(DeleteBehavior.Cascade)
                ;

            entity.HasOne(d => d.Opportunity).WithMany(p => p.SalesTasks)
                .OnDelete(DeleteBehavior.SetNull)
                ;

            entity.HasOne(d => d.Quote).WithMany(p => p.SalesTasks)
                .OnDelete(DeleteBehavior.SetNull)
                ;

            entity.HasOne(d => d.RelatedProduct).WithMany(p => p.SalesTasks)
                .OnDelete(DeleteBehavior.SetNull)
                ;
        
    }
}