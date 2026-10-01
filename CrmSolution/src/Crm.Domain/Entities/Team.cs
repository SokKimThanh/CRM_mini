using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class Team
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? ManagerId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
