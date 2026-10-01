using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class InteractionConfiguration : IEntityTypeConfiguration<Interaction>
{
    public void Configure(EntityTypeBuilder<Interaction> entity)
    {

            entity.HasKey(e => e.Id).HasName("interactions_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.InteractedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Contact).WithMany(p => p.Interactions)
                .OnDelete(DeleteBehavior.SetNull)
                ;

            entity.HasOne(d => d.Customer).WithMany(p => p.Interactions);

            entity.HasOne(d => d.Opportunity).WithMany(p => p.Interactions)
                .OnDelete(DeleteBehavior.SetNull)
                ;
        
    }
}