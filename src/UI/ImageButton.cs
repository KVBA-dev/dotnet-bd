using Raylib_cs;

namespace Game.UI;

public sealed class ImageButton : UIElement {
    public Texture2D Image { get; set; }
    public Rectangle ImageRect { get; set; }

    public event Action OnClick = () => { };
    public ImageButton(IUIHandler parent, Rectangle rect, Texture2D image) : base(parent, rect) {
        Image = image;
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
        Color col = palette.background;
        if (Input.MouseInRect(Rect)) {
            col = palette.backgroundSelected;
        }

        rl.DrawRectangleRec(Rect, col);
        rl.DrawTexturePro(Image, new(0, 0, Image.Width, Image.Height), ImageRect, new(0, 0), 0, Color.White);
    }
}