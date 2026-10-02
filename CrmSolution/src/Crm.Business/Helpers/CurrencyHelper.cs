using System.Globalization;

namespace Crm.Business.Helpers;

public static class CurrencyHelper
{
    private static readonly CultureInfo VietnamCulture = CultureInfo.GetCultureInfo("vi-VN");

    /// <summary>
    /// Định dạng đầy đủ (cho Desktop): 15000000 -> 15.000.000
    /// </summary>
    public static string Format(decimal amount)
        => amount.ToString("N0", VietnamCulture);

    /// <summary>
    /// Định dạng rút gọn (cho Mobile): 15000000 -> 15 tr
    /// </summary>
    public static string FormatShort(decimal amount)
    {
        var abs = Math.Abs(amount);
        var sign = amount < 0 ? "-" : "";

        if (abs >= 1_000_000_000m) return $"{sign}{abs / 1_000_000_000m:0.##} tỷ";
        if (abs >= 1_000_000m) return $"{sign}{abs / 1_000_000m:0.##} tr";
        if (abs >= 1_000m) return $"{sign}{abs / 1_000m:0.##}K";
        return amount.ToString("N0", VietnamCulture);
    }
}