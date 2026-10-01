using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> entity)
    {

            entity.HasKey(e => e.Id).HasName("opportunities_pkey");

            entity.HasIndex(e => e.AssignedToUserId, "idx_opp_assigned").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.StageId, "idx_opp_stage").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.IsWon, "idx_opp_won").HasFilter("(is_won = true)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.EstimatedValue).HasDefaultValue(0m);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Probability).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Contact).WithMany(p => p.Opportunities)
                .OnDelete(DeleteBehavior.SetNull)
                ;

            entity.HasOne(d => d.Customer).WithMany(p => p.Opportunities)
                .OnDelete(DeleteBehavior.Restrict)
                ;

            entity.HasOne(d => d.Stage).WithMany(p => p.Opportunities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                ;
        
    }
}