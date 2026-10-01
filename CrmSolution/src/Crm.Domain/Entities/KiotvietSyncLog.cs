using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("kiotviet_sync_logs")]
[Index("Status", Name = "idx_sync_logs_status")]
[Index("EntityType", "StartedAt", Name = "idx_sync_logs_type", IsDescending = new[] { false, true })]
public partial class KiotvietSyncLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("entity_type")]
    [StringLength(50)]
    public string EntityType { get; set; } = null!;

    [Column("action")]
    [StringLength(20)]
    public string? Action { get; set; }

    [Column("records_synced")]
    public int? RecordsSynced { get; set; }

    [Column("records_failed")]
    public int? RecordsFailed { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column("message")]
    public string? Message { get; set; }

    [Column("error_detail")]
    public string? ErrorDetail { get; set; }

    [Column("started_at")]
    public DateTime? StartedAt { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("duration_ms")]
    public int? DurationMs { get; set; }
}
