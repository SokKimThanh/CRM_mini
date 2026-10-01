using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class UserProfile
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string? EmployeeCode { get; set; }

    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }

    public string RoleCode { get; set; } = null!;

    public int? TeamId { get; set; }

    public decimal? MonthlyTarget { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Team? Team { get; set; }

    public virtual ApplicationUser? User { get; set; }
}
