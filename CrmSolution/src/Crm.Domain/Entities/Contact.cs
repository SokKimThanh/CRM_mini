using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("contacts")]
[Index("CustomerId", Name = "idx_contacts_customer")]
public partial class Contact
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("customer_id")]
    public long CustomerId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("position")]
    [StringLength(100)]
    public string? Position { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Column("birthday")]
    public DateOnly? Birthday { get; set; }

    [Column("is_primary")]
    public bool? IsPrimary { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Contacts")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("Contact")]
    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();

    [InverseProperty("Contact")]
    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();

    [InverseProperty("Contact")]
    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

    [InverseProperty("Contact")]
    public virtual ICollection<SalesTask> SalesTasks { get; set; } = new List<SalesTask>();
}
