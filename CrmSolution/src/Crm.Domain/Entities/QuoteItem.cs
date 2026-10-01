using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("quote_items")]
[Index("ProductId", Name = "idx_quote_items_product")]
[Index("QuoteId", Name = "idx_quote_items_quote")]
public partial class QuoteItem
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("quote_id")]
    public long QuoteId { get; set; }

    [Column("product_id")]
    public long? ProductId { get; set; }

    [Column("product_name")]
    [StringLength(255)]
    public string ProductName { get; set; } = null!;

    [Column("product_code")]
    [StringLength(50)]
    public string? ProductCode { get; set; }

    [Column("unit")]
    [StringLength(50)]
    public string? Unit { get; set; }

    [Column("quantity")]
    [Precision(12, 2)]
    public decimal Quantity { get; set; }

    [Column("unit_price")]
    [Precision(18, 2)]
    public decimal UnitPrice { get; set; }

    [Column("discount_amount")]
    [Precision(18, 2)]
    public decimal? DiscountAmount { get; set; }

    [Column("total_line")]
    [Precision(18, 2)]
    public decimal TotalLine { get; set; }

    [Column("sort_order")]
    public int? SortOrder { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("QuoteItems")]
    public virtual Product? Product { get; set; }

    [ForeignKey("QuoteId")]
    [InverseProperty("QuoteItems")]
    public virtual Quote Quote { get; set; } = null!;
}
