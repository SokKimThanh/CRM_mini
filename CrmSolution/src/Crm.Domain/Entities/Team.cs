using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public class Team
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ParentTeamId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }

    // [K16] Navigation tường minh — Bắt buộc khai báo đầy đủ collection đối ứng
    public Team? ParentTeam { get; set; }
    public ICollection<Team> ChildTeams { get; set; } = new List<Team>();
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
