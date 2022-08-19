using MudBlazor;

namespace BlazorGrpcWebApp.Client.ThemesProvider
{
    public static class MudThemeProvider
    {
        public static MudTheme CreateStartMudTheme()
        {
            return new MudTheme()
            {
                Palette = new Palette()
                {
                    // Box Lines + Font
                    Primary = "#c9d1d3",
                    Secondary = "#238636",
                    Tertiary = "#c9d1d3",

                    // Main Background
                    Background = "#0d1117",

                    // Paper Background
                    Surface = "#0d1117",

                    Success = "#32CD32ff",
                    TextPrimary = "#c9d1d3",
                    TextSecondary = "#c9d1d3",
                    ActionDefault = "#fff7",
                    ActionDisabled = "#fff7",
                    Warning = "#bb8800ff",
                    Info = "#53a6ff",
                    Error = "#53a6ff",
                },
                Typography = new Typography()
                {
                    Default = new Default()
                    {
                        FontFamily = new[] { "Roboto-Regular" }
                    }
                },
            };
        }
        public static MudTheme CreateLoginMudTheme()
        {
            return new MudTheme()
            {
                Palette = new Palette()
                {
                    // Box Lines + Font
                    Primary = "#c9d1d3",
                    Secondary = "#238636",
                    Tertiary = "#c9d1d3",

                    // Main Background
                    Background = "#0d1117",

                    // Paper Background
                    Surface = "#0d1117",

                    Success = "#32CD32ff",
                    TextPrimary = "#c9d1d3",
                    TextSecondary = "#c9d1d3",
                    ActionDefault = "#fff7",
                    ActionDisabled = "#fff7",
                    Warning = "#bb8800ff",
                    Info = "#53a6ff",
                    Error = "#53a6ff",
                },
                Typography = new Typography()
                {
                    Default = new Default()
                    {
                        FontFamily = new[] { "Roboto-Regular" }
                    }
                },
            };
        }
        public static MudTheme CreateMainMudTheme()
        {
            return new MudTheme()
            {
                Palette = new Palette()
                {
                    // Box Lines + Font
                    Primary = "#ffff",
                    Secondary = "#238636",
                    Tertiary = "#ff4081ff",

                    // Main Background
                    Background = "#010409",

                    // Paper Background
                    Surface = "#0d1117",

                    // Drawer Background
                    DrawerBackground = "#1a1a27ff",

                    // Appbar Background
                    AppbarBackground = "#161b22",

                    Success = "#32CD32ff",
                    TextPrimary = "#eeef",
                    TextSecondary = "#c9d1d3",
                    DrawerText = "#eeee",
                    DrawerIcon = "rgba(255,255,255, 1)",
                    TableHover = "#aaaf",
                    ActionDefault = "fff7",
                    ActionDisabled = "#fff7",
                    Warning = "#bb8800ff",
                    Error = "#53a6ff",
                },
                LayoutProperties = new LayoutProperties()
                {
                    DrawerWidthLeft = "14vw",
                    DrawerMiniWidthLeft = "65px",
                },

                Typography = new Typography()
                {
                    Default = new Default()
                    {
                        FontFamily = new[] { "Roboto-Regular" }
                    }
                },
            };
        }
    }
}
