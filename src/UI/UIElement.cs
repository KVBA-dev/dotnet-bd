using Raylib_cs;

namespace Game.UI;

public abstract class UIElement {
    public Rectangle Rect { get; set; }
    public bool Focused { get; set; }
    protected IUIHandler parent;
    public ColorPalette palette { get; set; } = ColorPalette.Default;

    public Action<UIElement> OnUIConfirm = _ => {};
    public Action<UIElement> OnUICancel = _ => {};

    public UIElement(IUIHandler parent, Rectangle rect) {
        Rect = rect;
        this.parent = parent;
    }

    public abstract bool Update();
    public abstract void Render();


}