using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class Contact
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Position { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateOnly? Birthday { get; set; }

    public bool? IsPrimary { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();

    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();

    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

    public virtual ICollection<SalesTask> SalesTasks { get; set; } = new List<SalesTask>();
}
