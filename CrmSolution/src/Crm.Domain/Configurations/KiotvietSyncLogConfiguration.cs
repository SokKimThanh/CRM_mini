using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class KiotvietSyncLogConfiguration : IEntityTypeConfiguration<KiotvietSyncLog>
{
    public void Configure(EntityTypeBuilder<KiotvietSyncLog> entity)
    {

            entity.HasKey(e => e.Id).HasName("kiotviet_sync_logs_pkey");

            entity.Property(e => e.RecordsFailed).HasDefaultValue(0);
            entity.Property(e => e.RecordsSynced).HasDefaultValue(0);
            entity.Property(e => e.StartedAt).HasDefaultValueSql("now()");
        
    }
}