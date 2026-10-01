using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("interactions")]
[Index("CustomerId", Name = "idx_interactions_customer")]
[Index("InteractedAt", Name = "idx_interactions_date", AllDescending = true)]
[Index("UserId", Name = "idx_interactions_user")]
public partial class Interaction
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("customer_id")]
    public long CustomerId { get; set; }

    [Column("contact_id")]
    public long? ContactId { get; set; }

    [Column("opportunity_id")]
    public long? OpportunityId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("type")]
    public int Type { get; set; }

    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("duration_minutes")]
    public int? DurationMinutes { get; set; }

    [Column("outcome")]
    public int? Outcome { get; set; }

    [Column("interacted_at")]
    public DateTime InteractedAt { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("ContactId")]
    [InverseProperty("Interactions")]
    public virtual Contact? Contact { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Interactions")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("OpportunityId")]
    [InverseProperty("Interactions")]
    public virtual Opportunity? Opportunity { get; set; }
}
