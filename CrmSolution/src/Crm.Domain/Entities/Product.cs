using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("products")]
[Index("CategoryId", Name = "idx_products_category")]
[Index("Code", Name = "idx_products_code")]
[Index("KiotvietId", Name = "idx_products_kiotviet")]
[Index("KiotvietId", Name = "products_kiotviet_id_key", IsUnique = true)]
public partial class Product
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("kiotviet_id")]
    public long? KiotvietId { get; set; }

    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("category_id")]
    public long? CategoryId { get; set; }

    [Column("unit")]
    [StringLength(50)]
    public string? Unit { get; set; }

    [Column("base_price")]
    [Precision(18, 2)]
    public decimal? BasePrice { get; set; }

    [Column("cost_price")]
    [Precision(18, 2)]
    public decimal? CostPrice { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<QuoteItem> QuoteItems { get; set; } = new List<QuoteItem>();

    [InverseProperty("RelatedProduct")]
    public virtual ICollection<SalesTask> SalesTasks { get; set; } = new List<SalesTask>();
}
