using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class DashboardSnapshotConfiguration : IEntityTypeConfiguration<DashboardSnapshot>
{
    public void Configure(EntityTypeBuilder<DashboardSnapshot> entity)
    {

            entity.HasKey(e => e.Id).HasName("dashboard_snapshots_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.NewCustomers).HasDefaultValue(0);
            entity.Property(e => e.OldCustomers).HasDefaultValue(0);
            entity.Property(e => e.TotalOppValue).HasDefaultValue(0m);
            entity.Property(e => e.TotalOpportunities).HasDefaultValue(0);
            entity.Property(e => e.TotalRevenue).HasDefaultValue(0m);
            entity.Property(e => e.WinRate).HasDefaultValue(0m);
            entity.Property(e => e.WonOpportunities).HasDefaultValue(0);
        
    }
}