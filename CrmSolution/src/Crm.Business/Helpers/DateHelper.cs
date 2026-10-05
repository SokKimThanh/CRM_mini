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
        var normalizedUtc = utc.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(utc, DateTimeKind.Utc)
            : utc.ToUniversalTime();
        return TimeZoneInfo.ConvertTimeFromUtc(normalizedUtc, VietnamTz);
    }

    public static DateTime ToUtc(DateTime vietnam)
        => TimeZoneInfo.ConvertTimeToUtc(vietnam, VietnamTz);

    public static int DaysBetween(DateTime from, DateTime to)
        => (to.Date - from.Date).Days;
}