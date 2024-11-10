using Raylib_cs;
using System.Text;
namespace Game.UI;

public class InputField : UIElement {
    private static InputField? activeField = null;
    public InputField(IUIHandler parent, Rectangle rect) : base(parent, rect) {
    }
    private StringBuilder stringBuilder = new();
    public string Text {
        get => stringBuilder.ToString();
        set => stringBuilder = new(value);
    }
    public event Action<string> OnEditingEnd;

    public bool IsActive => ReferenceEquals(activeField, this);
    public Alignment Alignment { get; set; } = Alignment.CenterLeft;
    public int TextSize { get; set; } = 15;

    public override void Render() {
        if (IsActive) {
            rl.DrawRectangleRec(Rect, Color.Blue);
        }
        rl.DrawRectangleLinesEx(Rect, 3 * UISpecs.Scale, Color.SkyBlue);
        int textWidth = rl.MeasureText(Text, (int)(TextSize * UISpecs.Scale));
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

        rl.DrawText(Text, x, y, (int)(TextSize * UISpecs.Scale), Color.White);
    }

    public override void Update() {
        if (rl.IsMouseButtonPressed(MouseButton.Left)) {
            if (IsActive) {
                OnEditingEnd?.Invoke(Text);
            }
            if (rl.CheckCollisionPointRec(rl.GetMousePosition(), Rect)) {
                activeField = this;
                parent.SetFocused(this);
            }
            else if (activeField == this) {
                activeField = null;
            }
        }

        if (!IsActive) {
            return;
        }
        int key = rl.GetKeyPressed();

        if (key >= 32 && key <= 125) {
            stringBuilder.Append((char)key);
        }
        if (rl.IsKeyPressed(KeyboardKey.Backspace) && stringBuilder.Length > 0) {
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }
    }

}