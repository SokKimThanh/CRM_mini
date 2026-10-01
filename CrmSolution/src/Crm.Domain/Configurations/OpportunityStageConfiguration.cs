using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class OpportunityStageConfiguration : IEntityTypeConfiguration<OpportunityStage>
{
    public void Configure(EntityTypeBuilder<OpportunityStage> entity)
    {

            entity.HasKey(e => e.Id).HasName("opportunity_stages_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsClosed).HasDefaultValue(false);
            entity.Property(e => e.IsLost).HasDefaultValue(false);
            entity.Property(e => e.IsWon).HasDefaultValue(false);
        
    }
}