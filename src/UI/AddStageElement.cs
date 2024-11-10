using Raylib_cs;

namespace Game.UI;

public sealed class AddStageElement : UIElement, IStageSelectorElement {
    private Action action;
    private Label lbl;
    public AddStageElement(IUIHandler parent, Rectangle rect) : base(parent, rect) {
        action = () => { };
        lbl = new(parent, Rect, "+");
        lbl.TextSize = 40;
        lbl.Color = rl.ColorBrightness(Color.DarkBlue, -.2f);
    }

    public IStageSelectorElement OnClick(Action action) {
        this.action = action;
        return this;
    }

    public override void Render() {
        rl.DrawRectangleRec(Rect, Color.DarkBlue);
        lbl.Render();
    }

    public override void Update() {
        lbl.Rect = Rect;
        if (rl.IsMouseButtonPressed(MouseButton.Left) && rl.CheckCollisionPointRec(rl.GetMousePosition(), Rect)) {
            action?.Invoke();
        }
    }
}