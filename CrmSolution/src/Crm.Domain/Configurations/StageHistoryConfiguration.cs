using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class StageHistoryConfiguration : IEntityTypeConfiguration<StageHistory>
{
    public void Configure(EntityTypeBuilder<StageHistory> entity)
    {

        entity.HasKey(e => e.Id).HasName("stage_histories_pkey");

        entity.Property(e => e.ChangedAt).HasDefaultValueSql("now()");

        entity.HasOne(d => d.FromStage).WithMany(p => p.StageHistoryFromStages)
            .HasForeignKey(d => d.FromStageId);

        entity.HasOne(d => d.Opportunity).WithMany(p => p.StageHistories)
            .HasForeignKey(d => d.OpportunityId);

        entity.HasOne(d => d.ToStage).WithMany(p => p.StageHistoryToStages)
            .HasForeignKey(d => d.ToStageId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            ;

    }
}