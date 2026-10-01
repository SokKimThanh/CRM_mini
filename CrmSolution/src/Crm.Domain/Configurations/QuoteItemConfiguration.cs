using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class QuoteItemConfiguration : IEntityTypeConfiguration<QuoteItem>
{
    public void Configure(EntityTypeBuilder<QuoteItem> entity)
    {

            entity.HasKey(e => e.Id).HasName("quote_items_pkey");

            entity.Property(e => e.DiscountAmount).HasDefaultValue(0m);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);

            entity.HasOne(d => d.Product).WithMany(p => p.QuoteItems)
                .OnDelete(DeleteBehavior.SetNull)
                ;

            entity.HasOne(d => d.Quote).WithMany(p => p.QuoteItems);
        
    }
}