namespace Crm.Business.Helpers;

public static class DateHelper
{
    private static readonly TimeZoneInfo VietnamTz = GetVietnamTimeZone();

    private static TimeZoneInfo GetVietnamTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
        }
    }

    public static DateTime ToVietnamTime(DateTime utc)
        => TimeZoneInfo.ConvertTimeFromUtc(utc, VietnamTz);

    public static DateTime ToUtc(DateTime vietnam)
        => TimeZoneInfo.ConvertTimeToUtc(vietnam, VietnamTz);

    public static int DaysBetween(DateTime from, DateTime to)
        => (to.Date - from.Date).Days;
}