using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("audit_logs")]
[Index("ChangedAt", Name = "idx_audit_changed", AllDescending = true)]
[Index("EntityName", "EntityId", Name = "idx_audit_entity")]
[Index("UserId", Name = "idx_audit_user")]
public partial class AuditLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("entity_name")]
    [StringLength(100)]
    public string EntityName { get; set; } = null!;

    [Column("entity_id")]
    [StringLength(50)]
    public string EntityId { get; set; } = null!;

    [Column("action")]
    [StringLength(20)]
    public string Action { get; set; } = null!;

    [Column("field_name")]
    [StringLength(100)]
    public string? FieldName { get; set; }

    [Column("old_value")]
    public string? OldValue { get; set; }

    [Column("new_value")]
    public string? NewValue { get; set; }

    [Column("old_values_json", TypeName = "jsonb")]
    public string? OldValuesJson { get; set; }

    [Column("new_values_json", TypeName = "jsonb")]
    public string? NewValuesJson { get; set; }

    [Column("ip_address")]
    [StringLength(45)]
    public string? IpAddress { get; set; }

    [Column("user_agent")]
    [StringLength(500)]
    public string? UserAgent { get; set; }

    [Column("changed_at")]
    public DateTime? ChangedAt { get; set; }
}
