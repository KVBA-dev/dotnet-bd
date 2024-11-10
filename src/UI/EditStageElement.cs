using Game.Levels;
using Raylib_cs;

namespace Game.UI;

public sealed class EditStageElement : StageSelectorElement {
    private Rectangle deleteRect;
    public Action OnDelete = () => { };
    public EditStageElement(StageSelector parent, Rectangle rect, Stage stage) : base(parent, rect) {
    }

    public override void Render() {
        deleteRect = Rect.RelativeRect(new(.8f, .8f, .2f, .2f));
        rl.DrawRectangleRec(Rect, Color.DarkBlue);
        rl.DrawRectangleRec(deleteRect, Color.Red);
        Label.DrawImmediate("x", deleteRect, Alignment.Center, 20, Color.White);
    }

    public override void Update() {
        deleteRect = Rect.RelativeRect(new(.8f, .8f, .2f, .2f));
        if (!rl.IsMouseButtonPressed(MouseButton.Left)) {
            return;
        } 
        if (rl.CheckCollisionPointRec(Input.MousePosition, deleteRect)) {
            OnDelete?.Invoke();
            return;
        }
        if (rl.CheckCollisionPointRec(Input.MousePosition, Rect)) {
            OnSelected?.Invoke();
        }
    }
}