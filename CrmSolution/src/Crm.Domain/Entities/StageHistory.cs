using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("stage_histories")]
[Index("OpportunityId", Name = "idx_history_opp")]
public partial class StageHistory
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("opportunity_id")]
    public long OpportunityId { get; set; }

    [Column("from_stage_id")]
    public int? FromStageId { get; set; }

    [Column("to_stage_id")]
    public int ToStageId { get; set; }

    [Column("changed_by_user_id")]
    public Guid? ChangedByUserId { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("changed_at")]
    public DateTime? ChangedAt { get; set; }

    [ForeignKey("FromStageId")]
    [InverseProperty("StageHistoryFromStages")]
    public virtual OpportunityStage? FromStage { get; set; }

    [ForeignKey("OpportunityId")]
    [InverseProperty("StageHistories")]
    public virtual Opportunity Opportunity { get; set; } = null!;

    [ForeignKey("ToStageId")]
    [InverseProperty("StageHistoryToStages")]
    public virtual OpportunityStage ToStage { get; set; } = null!;
}
