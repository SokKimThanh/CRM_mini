using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("quotes")]
[Index("Code", Name = "idx_quotes_code")]
[Index("OpportunityId", Name = "idx_quotes_opp")]
[Index("QuoteDate", Name = "idx_quotes_quote_date", AllDescending = true)]
[Index("Code", Name = "quotes_code_key", IsUnique = true)]
public partial class Quote
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Column("opportunity_id")]
    public long? OpportunityId { get; set; }

    [Column("customer_id")]
    public long CustomerId { get; set; }

    [Column("contact_id")]
    public long? ContactId { get; set; }

    [Column("created_by_user_id")]
    public Guid? CreatedByUserId { get; set; }

    [Column("quote_date")]
    public DateOnly QuoteDate { get; set; }

    [Column("valid_until")]
    public DateOnly ValidUntil { get; set; }

    [Column("status")]
    public int Status { get; set; }

    [Column("sub_total")]
    [Precision(18, 2)]
    public decimal? SubTotal { get; set; }

    [Column("discount_amount")]
    [Precision(18, 2)]
    public decimal? DiscountAmount { get; set; }

    [Column("vat_rate")]
    [Precision(5, 2)]
    public decimal? VatRate { get; set; }

    [Column("vat_amount")]
    [Precision(18, 2)]
    public decimal? VatAmount { get; set; }

    [Column("total_amount")]
    [Precision(18, 2)]
    public decimal? TotalAmount { get; set; }

    [Column("payment_terms")]
    public string? PaymentTerms { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("approved_by")]
    public Guid? ApprovedBy { get; set; }

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ContactId")]
    [InverseProperty("Quotes")]
    public virtual Contact? Contact { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Quotes")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("OpportunityId")]
    [InverseProperty("Quotes")]
    public virtual Opportunity? Opportunity { get; set; }

    [InverseProperty("Quote")]
    public virtual ICollection<QuoteItem> QuoteItems { get; set; } = new List<QuoteItem>();

    [InverseProperty("Quote")]
    public virtual ICollection<SalesTask> SalesTasks { get; set; } = new List<SalesTask>();
}
