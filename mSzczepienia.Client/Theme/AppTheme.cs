using MudBlazor;

namespace mSzczepienia.Client.Theme;

public class MyAppTheme : MudTheme
{
    public MyAppTheme()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#0052a5",
            Secondary = "#D4213D",
            Success = "#009205",
            Error = "#FF0000",
            Info = "#B8DEF5",

            Background = "#F5F5F5",
            Surface = "#FFFFFF",
            DrawerBackground = "#FFFFFF",

            TextPrimary = "#424242",
            TextSecondary = "#9E9E9E",
            LinesDefault = "#BDBDBD",
        };

        PaletteDark = new PaletteDark()
        {
            Primary = "#4C6EF5",
            Secondary = "#D4213D",
            Success = "#009205",
            Error = "#FF0000",
            Info = "#B8DEF5",

            TextPrimary = "#F5F5F5",
            TextSecondary = "#BDBDBD",
            LinesDefault = "#9E9E9E",
        };
    }
}
