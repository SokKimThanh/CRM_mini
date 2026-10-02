using MudBlazor;

namespace Crm.Web.Theme;

public static class CrmTheme
{
    public static readonly MudTheme Current = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#667eea",
            Secondary = "#764ba2",
            AppbarBackground = "#667eea",
            Background = "#f8fafc",
            Surface = "#ffffff",
            DrawerBackground = "#ffffff",
            DrawerText = "#4a5568",
            AppbarText = "#ffffff"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#818cf8",
            Secondary = "#a78bfa",
            AppbarBackground = "#1f2937",
            Background = "#111827",
            Surface = "#1f2937",
            DrawerBackground = "#1f2937",
            DrawerText = "#e5e7eb",
            AppbarText = "#f9fafb"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "6px"
        }
    };
}