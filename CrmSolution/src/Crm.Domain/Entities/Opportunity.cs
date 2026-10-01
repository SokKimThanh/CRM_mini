using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("opportunities")]
[Index("CreatedAt", Name = "idx_opp_created")]
[Index("CustomerId", Name = "idx_opp_customer")]
[Index("ExpectedCloseDate", Name = "idx_opp_expected")]
[Index("Code", Name = "opportunities_code_key", IsUnique = true)]
public partial class Opportunity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Column("customer_id")]
    public long CustomerId { get; set; }

    [Column("contact_id")]
    public long? ContactId { get; set; }

    [Column("stage_id")]
    public int StageId { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("estimated_value")]
    [Precision(18, 2)]
    public decimal? EstimatedValue { get; set; }

    [Column("probability")]
    public int? Probability { get; set; }

    [Column("assigned_to_user_id")]
    public Guid? AssignedToUserId { get; set; }

    [Column("expected_close_date")]
    public DateTime? ExpectedCloseDate { get; set; }

    [Column("actual_close_date")]
    public DateTime? ActualCloseDate { get; set; }

    [Column("source")]
    [StringLength(100)]
    public string? Source { get; set; }

    [Column("is_won")]
    public bool? IsWon { get; set; }

    [Column("loss_reason")]
    public int? LossReason { get; set; }

    [Column("loss_note")]
    public string? LossNote { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ContactId")]
    [InverseProperty("Opportunities")]
    public virtual Contact? Contact { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Opportunities")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("Opportunity")]
    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();

    [InverseProperty("Opportunity")]
    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

    [InverseProperty("Opportunity")]
    public virtual ICollection<SalesTask> SalesTasks { get; set; } = new List<SalesTask>();

    [ForeignKey("StageId")]
    [InverseProperty("Opportunities")]
    public virtual OpportunityStage Stage { get; set; } = null!;

    [InverseProperty("Opportunity")]
    public virtual ICollection<StageHistory> StageHistories { get; set; } = new List<StageHistory>();
}
