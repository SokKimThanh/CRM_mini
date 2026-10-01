using System;
using System.Collections.Generic;

namespace Crm.Domain.Entities;

public partial class DashboardSnapshot
{
    public long Id { get; set; }

    public DateOnly SnapshotDate { get; set; }

    public DateOnly PeriodFrom { get; set; }

    public DateOnly PeriodTo { get; set; }

    public decimal? TotalRevenue { get; set; }

    public int? NewCustomers { get; set; }

    public int? OldCustomers { get; set; }

    public decimal? TotalOppValue { get; set; }

    public int? WonOpportunities { get; set; }

    public int? TotalOpportunities { get; set; }

    public decimal? WinRate { get; set; }

    public string? CustomerRatioJson { get; set; }

    public string? FunnelJson { get; set; }

    public string? MonthlyRevenueJson { get; set; }

    public string? TopStaffJson { get; set; }

    public string? AlertsJson { get; set; }

    public DateTime? CreatedAt { get; set; }
}
