using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> entity)
    {

            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.HasIndex(e => new { e.UserId, e.IsRead }, "idx_notif_user_unread").HasFilter("(is_read = false)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Priority).HasDefaultValue(1);
            entity.Property(e => e.Type).HasDefaultValue(1);
        
    }
}