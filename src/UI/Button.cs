using Raylib_cs;

namespace Game.UI;

public class Button : UIElement
{
    public string Caption
    {
        get => lbl.Caption;
        set => lbl.Caption = value;
    }
    public event Action OnClick;
    private Label lbl;
    public Button(IUIHandler parent, Rectangle rect, string caption) : base(parent, rect)
    {
        lbl = new(parent, rect, caption);
        lbl.Alignment = Alignment.Center;
        OnClick = () => { };
        OnUIConfirm = _ => OnClick?.Invoke();
    }

    public override bool Update()
    {
        lbl.Rect = Rect;
        if (rl.CheckCollisionPointRec(rl.GetMousePosition(), Rect))
        {
            if (rl.IsMouseButtonPressed(MouseButton.Left))
            {
                OnClick?.Invoke();
            }
            return true;
        }
        return false;
    }

    public override void Render()
    {
        Color col = palette.background;
        if (Input.MouseInRect(Rect))
        {
            col = palette.backgroundSelected;
        }
        rl.DrawRectangleRec(Rect, col);
        lbl.Render();
    }

    public Button OnClicked(Action action)
    {
        OnClick = action;
        return this;
    }

}
