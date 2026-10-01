using System.Globalization;

namespace Crm.Business.Helpers;

public static class CurrencyHelper
{
    private static readonly CultureInfo VietnamCulture = CultureInfo.GetCultureInfo("vi-VN");

    public static string Format(decimal amount)
        => amount.ToString("N0", VietnamCulture) + " đ";

    public static string FormatShort(decimal amount)
    {
        if (amount >= 1_000_000_000) return $"{amount / 1_000_000_000:0.##} tỷ";
        if (amount >= 1_000_000) return $"{amount / 1_000_000:0.##} tr";
        if (amount >= 1_000) return $"{amount / 1_000:0.##}K";
        return amount.ToString("N0", VietnamCulture) + " đ";
    }
}