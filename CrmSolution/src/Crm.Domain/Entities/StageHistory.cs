using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class StageHistory
{
    public long Id { get; set; }

    public long OpportunityId { get; set; }

    public int? FromStageId { get; set; }

    public int ToStageId { get; set; }

    public Guid? ChangedByUserId { get; set; }

    public string? Note { get; set; }

    public DateTime? ChangedAt { get; set; }

    public virtual OpportunityStage? FromStage { get; set; }

    public virtual Opportunity Opportunity { get; set; } = null!;

    public virtual OpportunityStage ToStage { get; set; } = null!;
}
