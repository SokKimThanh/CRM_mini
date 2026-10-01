using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("customer_assignments")]
[Index("CustomerId", Name = "idx_assign_customer")]
[Index("ToUserId", Name = "idx_assign_user")]
public partial class CustomerAssignment
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("customer_id")]
    public long CustomerId { get; set; }

    [Column("from_user_id")]
    public Guid? FromUserId { get; set; }

    [Column("to_user_id")]
    public Guid? ToUserId { get; set; }

    [Column("assigned_by")]
    public Guid? AssignedBy { get; set; }

    [Column("reason")]
    public string? Reason { get; set; }

    [Column("assigned_at")]
    public DateTime? AssignedAt { get; set; }

    [Column("unassigned_at")]
    public DateTime? UnassignedAt { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("CustomerAssignments")]
    public virtual Customer Customer { get; set; } = null!;
}
