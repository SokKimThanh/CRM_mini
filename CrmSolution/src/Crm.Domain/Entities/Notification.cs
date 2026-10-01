using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class Notification
{
    public long Id { get; set; }

    public Guid? UserId { get; set; }

    public int Type { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? RefType { get; set; }

    public long? RefId { get; set; }

    public string? LinkUrl { get; set; }

    public int? Priority { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }
}
