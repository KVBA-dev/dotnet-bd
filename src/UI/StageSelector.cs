using Game.Levels;
using Raylib_cs;

namespace Game.UI;

public sealed class StageSelector : UIElement {
    private readonly Level level;
    private static readonly Color bgCol = rl.ColorBrightness(Color.DarkBlue, -.2f);
    private static readonly Color selectCol = rl.ColorAlpha(Color.SkyBlue, .5f);
    public event Action<int> OnStageSelected;
    private int position = 0;


    private Rectangle rectNext, rectPrevious;
    private bool mouseOnNext, mouseOnPrev;
    private Label lbl_next, lbl_prev;

    private IEnumerable<StageSelectorElement> selectorElements;
    private bool isDirty = false;
    private Stack<int> stagesForDeletion = new();

    public StageSelector(IUIHandler parent, Rectangle rect, Level level) : base(parent, rect) {
        this.level = level;
        OnStageSelected = _ => { };

        lbl_next = new(parent, new(), ">") { Alignment = Alignment.Center, TextSize = 50 };
        lbl_prev = new(parent, new(), "<") { Alignment = Alignment.Center, TextSize = 50 };

        selectorElements = GetElements();

    }

    public override void Update() {
        if (isDirty) {
            while (stagesForDeletion.Count > 0) {
                level.stages.RemoveAt(stagesForDeletion.Pop());
            }
            selectorElements = GetElements();
            isDirty = false;
        }
        rectPrevious = Rect.RelativeRect(new(-.05f, 0, .05f, 1f));
        rectNext = Rect.RelativeRect(new(1f, 0, .05f, 1f));

        mouseOnNext = rl.CheckCollisionPointRec(Input.MousePosition, rectNext);
        mouseOnPrev = rl.CheckCollisionPointRec(Input.MousePosition, rectPrevious);

        lbl_next.Rect = rectNext;
        lbl_prev.Rect = rectPrevious;

        foreach (StageSelectorElement e in selectorElements) {
            e.Update();
        }

        if (!rl.IsMouseButtonPressed(MouseButton.Left)) {
            return;
        }
        if (mouseOnNext) {
            position += 1;
            if (position > level.stages.Count) {
                position = level.stages.Count;
            }
        }
        if (mouseOnPrev) {
            position -= 1;
            if (position < 0) {
                position = 0;
            }
        }
    }

    public override void Render() {
        rl.DrawRectangleRec(Rect, bgCol);

        if (mouseOnNext) {
            rl.DrawRectangleRec(rectNext, selectCol);
        }

        if (mouseOnPrev) {
            rl.DrawRectangleRec(rectPrevious, selectCol);
        }
        lbl_next.Render();
        lbl_prev.Render();

        foreach (StageSelectorElement e in selectorElements) {
            e.Render();
        }
    }

    private IEnumerable<StageSelectorElement> GetElements() {
        Rectangle baseRect = new(0.025f, 0.1f, .2f, .8f);
        int idx = position;
        for (int i = 0; i < 4; i++) {
            if (idx == level.stages.Count) {
                AddStageElement addElem = new (this, Rect.RelativeRect(baseRect));
                addElem.OnSelected += () => {
                    level.stages.Add(new());
                };
                yield return addElem;
                yield break;
            }
            EditStageElement editElem = new(this, Rect.RelativeRect(baseRect), level.stages[idx]);
            editElem.OnSelected += () => {
                OnStageSelected?.Invoke(idx);
            };
            editElem.OnDelete += () => {
                stagesForDeletion.Push(idx);
                isDirty = true;
            };
            yield return editElem;
            baseRect.X += .25f;
            idx++;
        }
    }
}