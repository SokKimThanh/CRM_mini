using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class Interaction
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public long? ContactId { get; set; }

    public long? OpportunityId { get; set; }

    public Guid? UserId { get; set; }

    public int Type { get; set; }

    public string Content { get; set; } = null!;

    public int? DurationMinutes { get; set; }

    public int? Outcome { get; set; }

    public DateTime InteractedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Contact? Contact { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Opportunity? Opportunity { get; set; }
}
