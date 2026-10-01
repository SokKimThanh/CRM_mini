using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class Quote
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public long? OpportunityId { get; set; }

    public long CustomerId { get; set; }

    public long? ContactId { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateOnly QuoteDate { get; set; }

    public DateOnly ValidUntil { get; set; }

    public int Status { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? VatRate { get; set; }

    public decimal? VatAmount { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? PaymentTerms { get; set; }

    public string? Note { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Contact? Contact { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Opportunity? Opportunity { get; set; }

    public virtual ICollection<QuoteItem> QuoteItems { get; set; } = new List<QuoteItem>();

    public virtual ICollection<SalesTask> SalesTasks { get; set; } = new List<SalesTask>();
}
