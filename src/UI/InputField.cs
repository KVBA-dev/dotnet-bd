using Raylib_cs;
using System.Text;
namespace Game.UI;

public class InputField : UIElement
{
    private static InputField? activeField = null;
    private Label lbl;
    public InputField(IUIHandler parent, Rectangle rect) : base(parent, rect)
    {
        lbl = new(parent, rect, "");
        Alignment = Alignment.CenterLeft;
    }
    private StringBuilder stringBuilder = new();
    public string Text
    {
        get => stringBuilder.ToString();
        set => stringBuilder = new(value);
    }
    public event Action<string> OnEditingEnd;

    public bool IsActive => ReferenceEquals(activeField, this);
    public bool HideContent { get; set; } = false;
    public Alignment Alignment
    {
        get => lbl.Alignment;
        set => lbl.Alignment = value;
    }
    public int TextSize { get; set; } = 15;

    public override void Render()
    {
        if (IsActive)
        {
            rl.DrawRectangleRec(Rect, palette.backgroundSelected);
        }
        else
        {
            rl.DrawRectangleRec(Rect, palette.background);
        }
        rl.DrawRectangleLinesEx(Rect, 3 * UISpecs.Scale, palette.border);

        string text;
        StringBuilder sb;
        if (HideContent)
        {
            sb = new("");
            sb.Append('*', Text.Length);
        }
        else
        {
            sb = new(Text);
        }
        if (IsActive)
        {
            sb.Append('|');
        }
        text = sb.ToString();

        int textWidth = rl.MeasureText(text, (int)(TextSize * UISpecs.Scale));
        Rectangle r = Rect;
        r.X = Rect.X + 4 * UISpecs.Scale;
        lbl.Rect = r;

        lbl.Caption = text;
        lbl.Render();
    }

    public override bool Update()
    {
        if (rl.IsMouseButtonPressed(MouseButton.Left))
        {
            if (IsActive)
            {
                OnEditingEnd?.Invoke(Text);
            }
            if (rl.CheckCollisionPointRec(rl.GetMousePosition(), Rect))
            {
                activeField = this;
                parent.SetFocused(this);
            }
            else if (activeField == this)
            {
                activeField = null;
            }
        }

        if (!IsActive)
        {
            return false;
        }
        int key = rl.GetCharPressed();

        if (key >= 32 && key <= 125)
        {
            stringBuilder.Append((char)key);
        }
        bool backspace = rl.IsKeyPressed(KeyboardKey.Backspace) || rl.IsKeyPressedRepeat(KeyboardKey.Backspace);
        if (backspace && stringBuilder.Length > 0)
        {
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }
        return activeField == this;
    }

}
