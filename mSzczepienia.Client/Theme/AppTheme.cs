using MudBlazor;

namespace mSzczepienia.Client.Theme;

public class MyAppTheme : MudTheme
{
    public MyAppTheme()
    {
        // Todo: adjust light theme colors
        PaletteLight = new PaletteLight()
        {
            Primary = "#5475F6",
            TextPrimary = "#000000",
            Success = "#009205",
            Error = "#FF0000",
        };

        // Todo: adjust dark theme colors
        PaletteDark = new PaletteDark()
        {
            Primary = "#5475F6",
            TextPrimary = "#FFFFFF",
            Success = "#009205",
            Error = "#FF0000",
        };

        //LayoutProperties = new LayoutProperties()
        //{
        //    DefaultBorderRadius = ,
        //};

        //Typography = new Typography()
        //{
        //    Default = new DefaultTypography()
        //    {
        //        FontFamily = ,
        //        FontWeight = ,
        //    },
        //    H6 = new H6Typography()
        //    {
        //        FontWeight = ,
        //    }
        //};
    }
}
