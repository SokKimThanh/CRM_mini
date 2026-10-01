using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class Customer
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Industry { get; set; }

    public string? TaxCode { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public int HealthStatus { get; set; }

    public int? AverageCycleDays { get; set; }

    public DateTime? LastOrderDate { get; set; }

    public DateTime? LastContactDate { get; set; }

    public DateTime? NextContactDue { get; set; }

    public decimal? Revenue90d { get; set; }

    public int? OrderCount90d { get; set; }

    public decimal? CurrentDebt { get; set; }

    public long? KiotvietId { get; set; }

    public string? Note { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<CustomerAssignment> CustomerAssignments { get; set; } = new List<CustomerAssignment>();

    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();

    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();

    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

    public virtual ICollection<SalesTask> SalesTasks { get; set; } = new List<SalesTask>();
}
