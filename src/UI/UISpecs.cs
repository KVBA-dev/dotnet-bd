using Raylib_cs;
using System.Numerics;

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

    public static Rectangle RelativeRect(this Rectangle parent, float x, float y, float width, float height) {
        return parent.RelativeRect(new(x, y, width, height));
    }

    public static Rectangle Rectangle(this Texture2D texture) {
        return new(0, 0, texture.Width, texture.Height);
    }

    public static Rectangle CenteredSquare(this Rectangle parent, float relativePadding = 0) {
        Assert.That(relativePadding >= 0 && relativePadding <= 1);
        Rectangle r = new();
        r.X = 0;
        r.Y = 0;
        if (parent.Width > parent.Height) {
            float relativeH = parent.Height / parent.Width;
            r.X = (1 - relativeH) / 2;

            r.X = r.X + relativePadding * (0.5f - r.X);
            r.Y = relativePadding * 0.5f;
            r.Width = relativeH - relativePadding * relativeH;
            r.Height = 1 - relativePadding;
            return parent.RelativeRect(r);
        }
        float relativeW = parent.Width / parent.Height;
        r.Y = (1 - relativeW) / 2;

        r.X = relativePadding * 0.5f;
        r.Y = r.Y + relativePadding * (0.5f - r.Y);
        r.Width = 1 - relativePadding;
        r.Height = relativeW - relativePadding * relativeW;
        return parent.RelativeRect(r);

    }

    public static Rectangle Padding(this Rectangle rect, float padding) {
        return new(rect.X + padding, rect.Y + padding, rect.Width - 2 * padding, rect.Height - 2 * padding);
    }

    public static Vector2 Center(this Rectangle rect) => rect.Position + (rect.Size / 2);
}
