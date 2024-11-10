using Raylib_cs;

namespace Game.UI;

public static class UISpecs {
    public static float Scale => rl.GetScreenWidth() / (float)Constants.REFERENCE_WIDTH;
    public static int Width => rl.GetScreenWidth();
    public static int Height => rl.GetScreenHeight();
    public static Rectangle ScreenRect => new(0, 0, Width, Height);

    public static Rectangle RelativeRect(this Rectangle parent, Rectangle child) {
        return new(
            parent.X + child.X * parent.Width,
            parent.Y + child.Y * parent.Height,
            parent.Width * child.Width,
            parent.Height * child.Height
        );
    }
}