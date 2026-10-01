using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("opportunity_stages")]
[Index("Name", Name = "opportunity_stages_name_key", IsUnique = true)]
public partial class OpportunityStage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("display_order")]
    public int DisplayOrder { get; set; }

    [Column("default_probability")]
    public int DefaultProbability { get; set; }

    [Column("color")]
    [StringLength(20)]
    public string? Color { get; set; }

    [Column("is_won")]
    public bool? IsWon { get; set; }

    [Column("is_lost")]
    public bool? IsLost { get; set; }

    [Column("is_closed")]
    public bool? IsClosed { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Stage")]
    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();

    [InverseProperty("FromStage")]
    public virtual ICollection<StageHistory> StageHistoryFromStages { get; set; } = new List<StageHistory>();

    [InverseProperty("ToStage")]
    public virtual ICollection<StageHistory> StageHistoryToStages { get; set; } = new List<StageHistory>();
}
