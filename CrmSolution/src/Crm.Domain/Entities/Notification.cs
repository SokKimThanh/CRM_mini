using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("notifications")]
[Index("CreatedAt", Name = "idx_notif_created", AllDescending = true)]
[Index("RefType", "RefId", Name = "idx_notif_ref")]
public partial class Notification
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("type")]
    public int Type { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("message")]
    public string Message { get; set; } = null!;

    [Column("ref_type")]
    [StringLength(50)]
    public string? RefType { get; set; }

    [Column("ref_id")]
    public long? RefId { get; set; }

    [Column("link_url")]
    [StringLength(500)]
    public string? LinkUrl { get; set; }

    [Column("priority")]
    public int? Priority { get; set; }

    [Column("is_read")]
    public bool? IsRead { get; set; }

    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
}
