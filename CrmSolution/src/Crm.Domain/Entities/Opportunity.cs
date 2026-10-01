using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class Opportunity
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public long CustomerId { get; set; }

    public long? ContactId { get; set; }

    public int StageId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? EstimatedValue { get; set; }

    public int? Probability { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public DateTime? ExpectedCloseDate { get; set; }

    public DateTime? ActualCloseDate { get; set; }

    public string? Source { get; set; }

    public bool? IsWon { get; set; }

    public int? LossReason { get; set; }

    public string? LossNote { get; set; }

    public string? Note { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Contact? Contact { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();

    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

    public virtual ICollection<SalesTask> SalesTasks { get; set; } = new List<SalesTask>();

    public virtual OpportunityStage Stage { get; set; } = null!;

    public virtual ICollection<StageHistory> StageHistories { get; set; } = new List<StageHistory>();
}
