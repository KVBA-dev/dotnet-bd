using Raylib_cs;

namespace Game.UI;

public sealed class AddStageElement : StageSelectorElement {
    private Label lbl;
    public AddStageElement(StageSelector parent, Rectangle rect) : base(parent, rect) {
        lbl = new(null, Rect, "+");
        lbl.TextSize = 40;
        lbl.Color = rl.ColorBrightness(Color.DarkBlue, -.2f);
    }

    public override void Render() {
        rl.DrawRectangleRec(Rect, Color.DarkBlue);
        lbl.Render();
    }

    public override void Update() {
        lbl.Rect = Rect;
        if (rl.IsMouseButtonPressed(MouseButton.Left) && rl.CheckCollisionPointRec(rl.GetMousePosition(), Rect)) {
            OnSelected?.Invoke();
        }
    }
}