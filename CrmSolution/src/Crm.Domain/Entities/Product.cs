using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class Product
{
    public long Id { get; set; }

    public long? KiotvietId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public long? CategoryId { get; set; }

    public string? Unit { get; set; }

    public decimal? BasePrice { get; set; }

    public decimal? CostPrice { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<QuoteItem> QuoteItems { get; set; } = new List<QuoteItem>();

    public virtual ICollection<SalesTask> SalesTasks { get; set; } = new List<SalesTask>();
}
