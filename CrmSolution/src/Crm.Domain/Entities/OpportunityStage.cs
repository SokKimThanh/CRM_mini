using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class OpportunityStage
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public int DefaultProbability { get; set; }

    public string? Color { get; set; }

    public bool? IsWon { get; set; }

    public bool? IsLost { get; set; }

    public bool? IsClosed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();

    public virtual ICollection<StageHistory> StageHistoryFromStages { get; set; } = new List<StageHistory>();

    public virtual ICollection<StageHistory> StageHistoryToStages { get; set; } = new List<StageHistory>();
}
