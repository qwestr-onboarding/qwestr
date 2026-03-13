using MudBlazor;

namespace QwestrWebClient.Shared;

public static class QwestrTheme
{
    public static readonly MudTheme Theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#6C3FC5",
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#FF6B35",
            SecondaryContrastText = "#FFFFFF",
            Background = "#F9FAFE",
            Surface = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#333333",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#424242",
            TextPrimary = "#333333",
            TextSecondary = "#757575",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Inter", "sans-serif" }
            }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px"
        }
    };
}
