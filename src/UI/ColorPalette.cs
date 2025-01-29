using Raylib_cs;

namespace Game.UI;

public struct ColorPalette {
    public Color background;
    public Color foreground;
    public Color border;
    public Color backgroundSelected;

    public static ColorPalette Default => new() {
        background = Color.DarkBlue,
        backgroundSelected = rl.ColorAlpha(Color.SkyBlue, .5f),
        foreground = Color.White,
        border = Color.SkyBlue,
    };

    public static ColorPalette Editor => new() {
        background = rl.ColorAlpha(Color.White, .2f),
        backgroundSelected = rl.ColorAlpha(Color.White, .5f),
        foreground = Color.White,
        border = Color.White,
    };
}