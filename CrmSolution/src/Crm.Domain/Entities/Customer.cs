using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("customers")]
[Index("Code", Name = "customers_code_key", IsUnique = true)]
[Index("KiotvietId", Name = "customers_kiotviet_id_key", IsUnique = true)]
[Index("Code", Name = "idx_customers_code")]
[Index("KiotvietId", Name = "idx_customers_kiotviet")]
public partial class Customer
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("industry")]
    [StringLength(100)]
    public string? Industry { get; set; }

    [Column("tax_code")]
    [StringLength(50)]
    public string? TaxCode { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Column("website")]
    [StringLength(200)]
    public string? Website { get; set; }

    [Column("assigned_to_user_id")]
    public Guid? AssignedToUserId { get; set; }

    [Column("health_status")]
    public int HealthStatus { get; set; }

    [Column("average_cycle_days")]
    public int? AverageCycleDays { get; set; }

    [Column("last_order_date")]
    public DateTime? LastOrderDate { get; set; }

    [Column("last_contact_date")]
    public DateTime? LastContactDate { get; set; }

    [Column("next_contact_due")]
    public DateTime? NextContactDue { get; set; }

    [Column("revenue_90d")]
    [Precision(18, 2)]
    public decimal? Revenue90d { get; set; }

    [Column("order_count_90d")]
    public int? OrderCount90d { get; set; }

    [Column("current_debt")]
    [Precision(18, 2)]
    public decimal? CurrentDebt { get; set; }

    [Column("kiotviet_id")]
    public long? KiotvietId { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    [InverseProperty("Customer")]
    public virtual ICollection<CustomerAssignment> CustomerAssignments { get; set; } = new List<CustomerAssignment>();

    [InverseProperty("Customer")]
    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();

    [InverseProperty("Customer")]
    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();

    [InverseProperty("Customer")]
    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

    [InverseProperty("Customer")]
    public virtual ICollection<SalesTask> SalesTasks { get; set; } = new List<SalesTask>();
}
