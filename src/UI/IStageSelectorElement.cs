using Raylib_cs;

namespace Game.UI;

public abstract class StageSelectorElement {
    protected StageSelector parent;
    public Rectangle Rect { get; set; }
    public Action OnSelected = () => { };
    public StageSelectorElement(StageSelector parent, Rectangle rect) {
        this.parent = parent;
        Rect = rect;
    }

    public void Select() {
        OnSelected?.Invoke();
    }

    public abstract void Render();
    public abstract void Update();
}