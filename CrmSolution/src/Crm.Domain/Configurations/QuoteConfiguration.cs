using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> entity)
    {

            entity.HasKey(e => e.Id).HasName("quotes_pkey");

            entity.HasIndex(e => e.CustomerId, "idx_quotes_customer").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.Status, "idx_quotes_status").HasFilter("(is_deleted = false)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.DiscountAmount).HasDefaultValue(0m);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.QuoteDate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.SubTotal).HasDefaultValue(0m);
            entity.Property(e => e.TotalAmount).HasDefaultValue(0m);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VatAmount).HasDefaultValue(0m);
            entity.Property(e => e.VatRate).HasDefaultValue(10m);

            entity.HasOne(d => d.Contact).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.SetNull)
                ;

            entity.HasOne(d => d.Customer).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                ;

            entity.HasOne(d => d.Opportunity).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.OpportunityId)
                .OnDelete(DeleteBehavior.SetNull)
                ;
        
    }
}