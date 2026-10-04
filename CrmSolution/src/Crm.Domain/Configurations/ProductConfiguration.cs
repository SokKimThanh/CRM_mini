using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> entity)
    {

            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.HasIndex(e => e.IsActive, "idx_products_active").HasFilter("(is_active = true)");

            entity.Property(e => e.BasePrice).HasDefaultValue(0m);
            entity.Property(e => e.CostPrice).HasDefaultValue(0m);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                ;
        
    }
}