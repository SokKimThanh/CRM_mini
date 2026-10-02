using System;

namespace Crm.Business.Helpers;

public static class DateHelper
{
    private static readonly TimeZoneInfo VietnamTz = ResolveVietnamTimeZone();

    private static TimeZoneInfo ResolveVietnamTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); } catch { }
        try { return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"); } catch { }
        return TimeZoneInfo.CreateCustomTimeZone("ICT", TimeSpan.FromHours(7), "Indochina Time", "ICT");
    }

    public static DateTime ToVietnamTime(DateTime utc)
    {
        var standardizedUtc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(standardizedUtc, VietnamTz);
    }

    public static DateTime ToUtc(DateTime vietnamTime)
    {
        var unspecified = DateTime.SpecifyKind(vietnamTime, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspecified, VietnamTz);
    }

    public static int DaysBetween(DateTime from, DateTime to)
        => (to.Date - from.Date).Days;
}
