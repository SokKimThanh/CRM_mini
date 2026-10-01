using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> entity)
    {

            entity.HasKey(e => e.Id).HasName("audit_logs_pkey");

            entity.Property(e => e.ChangedAt).HasDefaultValueSql("now()");
        
    }
}