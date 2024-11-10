using Raylib_cs;

namespace Game.UI;

public class Label : UIElement {
    public string Caption { get; set; }
    public int TextSize { get; set; } = 15;
    public Color Color { get; set; } = Color.White;
    public Alignment Alignment { get; set; } = Alignment.Center;
    public Label(IUIHandler parent, Rectangle rect, string caption) : base(parent, rect) {
        Caption = caption;
    }

    public override void Render() {
        int textWidth = rl.MeasureText(Caption, (int)(TextSize * UISpecs.Scale));
        int x = (int)Rect.X;
        int y = (int)Rect.Y;
        switch (Alignment) {
            case Alignment.CenterLeft:
            case Alignment.TopLeft:
            case Alignment.BottomLeft:
                break;
            case Alignment.TopCenter:
            case Alignment.Center:
            case Alignment.BottomCenter:
                x += (int)(Rect.Width - textWidth) / 2;
                break;
            case Alignment.CenterRight:
            case Alignment.TopRight:
            case Alignment.BottomRight:
                x += (int)(Rect.Width - textWidth);
                break;
            default:
                break;
        };

        switch (Alignment) {
            case Alignment.TopCenter:
            case Alignment.TopLeft:
            case Alignment.TopRight:
                break;
            case Alignment.CenterLeft:
            case Alignment.Center:
            case Alignment.CenterRight:
                y += (int)(Rect.Height - TextSize * UISpecs.Scale) / 2;
                break;
            case Alignment.BottomCenter:
            case Alignment.BottomLeft:
            case Alignment.BottomRight:
                y += (int)(Rect.Height - TextSize * UISpecs.Scale);
                break;
            default:
                break;
        }

        rl.DrawText(Caption, x, y, (int)(TextSize * UISpecs.Scale), Color);
    }
}