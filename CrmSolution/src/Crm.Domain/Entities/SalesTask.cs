using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class SalesTask
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int Type { get; set; }

    public int Priority { get; set; }

    public int Status { get; set; }

    public long? CustomerId { get; set; }

    public long? ContactId { get; set; }

    public long? OpportunityId { get; set; }

    public long? QuoteId { get; set; }

    public long? RelatedProductId { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public Guid? CompletedBy { get; set; }

    public int? Outcome { get; set; }

    public string? ResultNote { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Contact? Contact { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Opportunity? Opportunity { get; set; }

    public virtual Quote? Quote { get; set; }

    public virtual Product? RelatedProduct { get; set; }
}
