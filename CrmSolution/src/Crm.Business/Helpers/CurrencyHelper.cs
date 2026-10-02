using System.Globalization;

namespace Crm.Business.Helpers;

public static class CurrencyHelper
{
    private static readonly CultureInfo VietnamCulture = CultureInfo.GetCultureInfo("vi-VN");

    public static string Format(decimal amount)
        => amount.ToString("N0", VietnamCulture);

    public static string FormatShort(decimal amount)
    {
        var absAmount = Math.Abs(amount);
        var sign = amount < 0 ? "-" : "";

        if (absAmount >= 1_000_000_000m)
            return $"{sign}{(absAmount / 1_000_000_000m).ToString("0.##", VietnamCulture)} tỷ";
        if (absAmount >= 1_000_000m)
            return $"{sign}{(absAmount / 1_000_000m).ToString("0.##", VietnamCulture)} tr";
        if (absAmount >= 1_000m)
            return $"{sign}{(absAmount / 1_000m).ToString("0.##", VietnamCulture)}K";

        return amount.ToString("N0", VietnamCulture);
    }
}
