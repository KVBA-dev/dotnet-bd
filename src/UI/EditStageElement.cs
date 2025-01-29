using Game.Levels;
using Raylib_cs;

namespace Game.UI;


public sealed class EditStageElement : StageSelectorElement {
    private Rectangle deleteRect;
    private Rectangle? nextRect, prevRect;
    private readonly Stage stage;
    public Action OnDelete = () => { };
    public Action OnMoveNext = () => { };
    public Action OnMovePrevious = () => { };
    public EditStageElement(StageSelector parent, Rectangle rect, Stage stage) : base(parent, rect) {
        this.stage = stage;
        nextRect = new();
        prevRect = new();
    }

    public EditStageElement First() {
        prevRect = null;
        OnMovePrevious = () => { };
        return this;
    }

    public EditStageElement Last() {
        nextRect = null;
        OnMoveNext = () => { };
        return this;
    }

    public override void Render() {
        CalculateRectangles();
        Rectangle lblRect = Rect.RelativeRect(new(.1f, .1f, .8f, .8f));
        rl.DrawRectangleRec(Rect, Color.DarkBlue);
        rl.DrawRectangleRec(deleteRect, Color.Red);
        if (nextRect is not null) {
            rl.DrawRectangleRec((Rectangle)nextRect, Constants.SelectedColour);
            Label.DrawImmediate(">", (Rectangle)nextRect, Alignment.Center, 10, Color.White);
        }
        if (prevRect is not null) {
            rl.DrawRectangleRec((Rectangle)prevRect, Constants.SelectedColour);
            Label.DrawImmediate("<", (Rectangle)prevRect, Alignment.Center, 10, Color.White);
        }
        Label.DrawImmediate("x", deleteRect, Alignment.Center, 20, Color.White);
        Label.DrawImmediate(stage.name, lblRect, Alignment.Center, 20, Color.White);
    }

    public override void Update() {
        CalculateRectangles();

        if (!rl.IsMouseButtonPressed(MouseButton.Left)) {
            return;
        } 
        if (rl.CheckCollisionPointRec(Input.MousePosition, deleteRect)) {
            OnDelete?.Invoke();
            return;
        }
        if (nextRect is not null && rl.CheckCollisionPointRec(Input.MousePosition, (Rectangle)nextRect)) {
            OnMoveNext?.Invoke();
            return;
        }
        if (prevRect is not null && rl.CheckCollisionPointRec(Input.MousePosition, (Rectangle)prevRect)) {
            OnMovePrevious?.Invoke();
            return;
        }
        if (rl.CheckCollisionPointRec(Input.MousePosition, Rect)) {
            OnSelected?.Invoke();
        }
    }

    private void CalculateRectangles() {
        deleteRect = Rect.RelativeRect(new(.8f, .8f, .2f, .2f));
        if (nextRect is not null) {
            nextRect = Rect.RelativeRect(.8f, 0, .2f, .15f);
        }
        if (prevRect is not null) {
            prevRect = Rect.RelativeRect(0f, 0, .2f, .15f);
        }
    }
}