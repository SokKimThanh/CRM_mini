using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Domain.Configurations;

public class CustomerAssignmentConfiguration : IEntityTypeConfiguration<CustomerAssignment>
{
    public void Configure(EntityTypeBuilder<CustomerAssignment> entity)
    {

        entity.HasKey(e => e.Id).HasName("customer_assignments_pkey");

        entity.Property(e => e.AssignedAt).HasDefaultValueSql("now()");

        entity.HasOne(d => d.Customer).WithMany(p => p.CustomerAssignments)
            .HasForeignKey(d => d.CustomerId);

    }
}