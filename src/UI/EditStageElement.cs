using Game.Levels;
using Raylib_cs;

namespace Game.UI;

public sealed class EditStageElement : UIElement, IStageSelectorElement {
    private readonly Stage stage;
    private Action action;
    private Rectangle moveBarRect;
    private Rectangle deleteRect;
    public EditStageElement(IUIHandler parent, Rectangle rect, Stage stage) : base(parent, rect) {
        this.stage = stage;
    }

    public IStageSelectorElement OnClick(Action action) {
        this.action = action;
        return this;
    }
    public override void Render() {
        rl.DrawRectangleRec(Rect, Color.DarkBlue);
        moveBarRect = Rect.RelativeRect(new(0, 0, 1, 0.05f));
        deleteRect = Rect.RelativeRect(new(.8f, .8f, .2f, .2f));
        rl.DrawRectangleRec(moveBarRect, rl.ColorBrightness(Color.DarkBlue, -.35f));
        rl.DrawRectangleRec(deleteRect, Color.Red);
    }

    public override void Update() {
        if (rl.IsMouseButtonPressed(MouseButton.Left) && rl.CheckCollisionPointRec(rl.GetMousePosition(), Rect)) {
            action?.Invoke();
        }
    }
}