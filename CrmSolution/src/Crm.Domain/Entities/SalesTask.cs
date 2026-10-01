using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("sales_tasks")]
[Index("OpportunityId", Name = "idx_tasks_opp")]
public partial class SalesTask
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("type")]
    public int Type { get; set; }

    [Column("priority")]
    public int Priority { get; set; }

    [Column("status")]
    public int Status { get; set; }

    [Column("customer_id")]
    public long? CustomerId { get; set; }

    [Column("contact_id")]
    public long? ContactId { get; set; }

    [Column("opportunity_id")]
    public long? OpportunityId { get; set; }

    [Column("quote_id")]
    public long? QuoteId { get; set; }

    [Column("related_product_id")]
    public long? RelatedProductId { get; set; }

    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("assigned_to_user_id")]
    public Guid? AssignedToUserId { get; set; }

    [Column("completed_by")]
    public Guid? CompletedBy { get; set; }

    [Column("outcome")]
    public int? Outcome { get; set; }

    [Column("result_note")]
    public string? ResultNote { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ContactId")]
    [InverseProperty("SalesTasks")]
    public virtual Contact? Contact { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("SalesTasks")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("OpportunityId")]
    [InverseProperty("SalesTasks")]
    public virtual Opportunity? Opportunity { get; set; }

    [ForeignKey("QuoteId")]
    [InverseProperty("SalesTasks")]
    public virtual Quote? Quote { get; set; }

    [ForeignKey("RelatedProductId")]
    [InverseProperty("SalesTasks")]
    public virtual Product? RelatedProduct { get; set; }
}
