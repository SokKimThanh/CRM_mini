using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("categories")]
[Index("KiotvietId", Name = "categories_kiotviet_id_key", IsUnique = true)]
public partial class Category
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("kiotviet_id")]
    public long? KiotvietId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("display_order")]
    public int? DisplayOrder { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
