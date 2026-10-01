using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class KiotvietSyncLog
{
    public long Id { get; set; }

    public string EntityType { get; set; } = null!;

    public string? Action { get; set; }

    public int? RecordsSynced { get; set; }

    public int? RecordsFailed { get; set; }

    public string Status { get; set; } = null!;

    public string? Message { get; set; }

    public string? ErrorDetail { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? DurationMs { get; set; }
}




