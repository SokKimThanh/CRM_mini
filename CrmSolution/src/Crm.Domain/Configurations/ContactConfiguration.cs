using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> entity)
    {

            entity.HasKey(e => e.Id).HasName("contacts_pkey");

            entity.HasIndex(e => new { e.CustomerId, e.IsPrimary }, "idx_contacts_primary").HasFilter("(is_primary = true)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Customer).WithMany(p => p.Contacts);
        
    }
}