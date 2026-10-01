using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class CustomerAssignment
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public Guid? FromUserId { get; set; }

    public Guid? ToUserId { get; set; }

    public Guid? AssignedBy { get; set; }

    public string? Reason { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime? UnassignedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
