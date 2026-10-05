using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Crm.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid TenantId { get; set; }
    public Guid? TeamId { get; set; }
    public bool IsDeleted { get; set; }

    // [K16] Navigation tường minh
    public Team? Team { get; set; }
    public ICollection<Customer> OwnedCustomers { get; set; } = new List<Customer>();
}
