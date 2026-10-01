using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class QuoteItem
{
    public long Id { get; set; }

    public long QuoteId { get; set; }

    public long? ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductCode { get; set; }

    public string? Unit { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal TotalLine { get; set; }

    public int? SortOrder { get; set; }

    public string? Note { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Quote Quote { get; set; } = null!;
}
