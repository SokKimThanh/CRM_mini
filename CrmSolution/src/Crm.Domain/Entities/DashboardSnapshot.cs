using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crm.Domain.Entities;

[Table("dashboard_snapshots")]
[Index("SnapshotDate", "PeriodFrom", "PeriodTo", Name = "dashboard_snapshots_snapshot_date_period_from_period_to_key", IsUnique = true)]
[Index("SnapshotDate", Name = "idx_snapshot_date", AllDescending = true)]
public partial class DashboardSnapshot
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("snapshot_date")]
    public DateOnly SnapshotDate { get; set; }

    [Column("period_from")]
    public DateOnly PeriodFrom { get; set; }

    [Column("period_to")]
    public DateOnly PeriodTo { get; set; }

    [Column("total_revenue")]
    [Precision(18, 2)]
    public decimal? TotalRevenue { get; set; }

    [Column("new_customers")]
    public int? NewCustomers { get; set; }

    [Column("old_customers")]
    public int? OldCustomers { get; set; }

    [Column("total_opp_value")]
    [Precision(18, 2)]
    public decimal? TotalOppValue { get; set; }

    [Column("won_opportunities")]
    public int? WonOpportunities { get; set; }

    [Column("total_opportunities")]
    public int? TotalOpportunities { get; set; }

    [Column("win_rate")]
    [Precision(5, 2)]
    public decimal? WinRate { get; set; }

    [Column("customer_ratio_json", TypeName = "jsonb")]
    public string? CustomerRatioJson { get; set; }

    [Column("funnel_json", TypeName = "jsonb")]
    public string? FunnelJson { get; set; }

    [Column("monthly_revenue_json", TypeName = "jsonb")]
    public string? MonthlyRevenueJson { get; set; }

    [Column("top_staff_json", TypeName = "jsonb")]
    public string? TopStaffJson { get; set; }

    [Column("alerts_json", TypeName = "jsonb")]
    public string? AlertsJson { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
}
