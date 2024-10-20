using Raylib_cs;
using System.Numerics;

namespace Game.UI;

public abstract class UIElement {
    public Rectangle Rect { get; set; }
    public bool Focused { get; set; }
    protected IUIHandler parent;

    public Action<UIElement> OnUIConfirm = _ => {};
    public Action<UIElement> OnUICancel = _ => {};

    public UIElement(IUIHandler parent, Rectangle rect) {
        Rect = rect;
        this.parent = parent;
    }

    public virtual void Update() {}
    public virtual void Render() {}

    public bool MouseIntersects(bool worldSpace = false) => MouseIntersects(Rect, worldSpace);
    public bool MouseIntersects(Rectangle rect, bool worldSpace = false) {
        Vector2 mousePos = Input.MousePosition;
        bool x = mousePos.X >= rect.X && mousePos.X <= rect.X + rect.Width;
        bool y = mousePos.Y >= rect.Y && mousePos.Y <= rect.Y + rect.Height;
        return x && y;
    }

}