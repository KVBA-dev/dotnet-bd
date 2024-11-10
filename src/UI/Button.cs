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

    public override void Update() {
        if (rl.CheckCollisionPointRec(rl.GetMousePosition(), Rect)) {
            parent.SetFocused(this);
            if (rl.IsMouseButtonPressed(MouseButton.Left)) {
                OnClick?.Invoke();
            }
        }

    }

    public override void Render() {
        int textSize = (int)(15 * UISpecs.Scale);
        int textWidth = rl.MeasureText(Caption, textSize);
        int textX = (int)Rect.X + ((int)Rect.Width - textWidth) / 2;
        int textY = (int)Rect.Y + textSize / 3;

        Color col = Color.SkyBlue;
        if (Focused) {
            col = Color.White;
            rl.DrawRectangleRec(Rect, rl.ColorAlpha(Color.SkyBlue, .5f));
        }
        rl.DrawText(Caption, textX, textY, textSize, col);
    }

    public Button OnClicked(Action action) {
        OnClick = action;
        return this;
    }

}