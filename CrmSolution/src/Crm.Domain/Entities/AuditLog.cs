using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class AuditLog
{
    public long Id { get; set; }

    public Guid? UserId { get; set; }

    public string EntityName { get; set; } = null!;

    public string EntityId { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? FieldName { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? OldValuesJson { get; set; }

    public string? NewValuesJson { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? ChangedAt { get; set; }
}
