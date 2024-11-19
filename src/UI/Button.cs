using Raylib_cs;

namespace Game.UI;

public class Button : UIElement {
    public string Caption { get; set; }
    public event Action OnClick;
    public Button(IUIHandler parent, Rectangle rect, string caption) : base(parent, rect) {
        Caption = caption;
        OnClick = () => {};
        OnUIConfirm = _ => OnClick?.Invoke();
    }

    public override bool Update() {
        if (rl.CheckCollisionPointRec(rl.GetMousePosition(), Rect)) {
            if (rl.IsMouseButtonPressed(MouseButton.Left)) {
                OnClick?.Invoke();
            }
            return true;
        }
        return false;
    }

    public override void Render() {
        int textSize = (int)(15 * UISpecs.Scale);
        int textWidth = rl.MeasureText(Caption, textSize);
        int textX = (int)Rect.X + ((int)Rect.Width - textWidth) / 2;
        int textY = (int)Rect.Y + textSize / 3;

        Color col = palette.background;
        if (Input.MouseInRect(Rect)) {
            col = palette.backgroundSelected;
        }
        rl.DrawRectangleRec(Rect, col);
        rl.DrawText(Caption, textX, textY, textSize, palette.foreground);
    }

    public Button OnClicked(Action action) {
        OnClick = action;
        return this;
    }

}