using Game.Levels;
using Raylib_cs;

namespace Game.UI;

public sealed class StageSelector : UIElement, IUIHandler {
    private readonly List<Stage> stages;
    public List<Stage> Stages => stages;
    private Button next, previous;
    private int position;
    public event Action<int> OnStageSelected;
    private List<UIElement> elements;
    private IEnumerable<IStageSelectorElement> stageSelectorElements;
    public StageSelector(IUIHandler parent, Rectangle rect, Level level) : base(parent, rect) {
        stages = level.stages;
        position = 0;
        next = new(this, new(), ">");
        next.OnClick += () => {
            if (position < stages.Count) {
                position += 1;
                stageSelectorElements = GetElements();
            }
        };
        previous = new(this, new(), "<");
        previous.OnClick += () => {
            if (position > 0) {
                position -= 1;
                stageSelectorElements = GetElements();
            }
        };
        OnStageSelected = _ => {};
        elements = [
            next,
            previous,
        ];
        stageSelectorElements = GetElements();
    }
    public UIElement FocusedElement {
        get {
            UIElement elem;
            try {
                elem = elements.Single(e => e.Focused);
            }
            catch {
                elem = null;
            }
            return elem;
        }
    }

    public override void Render() {
        Color bg = rl.ColorBrightness(Color.DarkBlue, -0.2f);
        rl.DrawRectangleRec(Rect, bg);
        next.Render();
        previous.Render();
        foreach(IStageSelectorElement element in stageSelectorElements) {
            (element as UIElement)?.Render();
        }
    }

    public void SetFocused(UIElement element) {
        UIElement e = FocusedElement;
        if (e is not null) {
            e.Focused = false;
        }
        element.Focused = true;
    }

    public override void Update() {
        next.Rect = Rect.RelativeRect(new(1f, 0, 0.05f, 1));
        previous.Rect = Rect.RelativeRect(new(-0.05f, 0, 0.05f, 1));

        next.Update();
        previous.Update();
        foreach (IStageSelectorElement element in stageSelectorElements) {
            (element as UIElement)?.Update();
        }
    }

    private IEnumerable<IStageSelectorElement> GetElements() {
        Rectangle rect;
        int pos = -position;
        int i = -1;
        foreach (Stage stage in stages) {
            i += 1;
            if (pos < 0) {
                pos += 1;
                continue;
            }
            if (pos >= 4) {
                yield break;
            }

            rect = Rect.RelativeRect(new(0.025f + 0.25f * pos, 0.1f, 0.2f, 0.8f));
            pos += 1;
            yield return new EditStageElement(this, rect, stage).OnClick(() => {
                Console.WriteLine("Opening stage...");
                OnStageSelected?.Invoke(i);
            });
        }
        rect = Rect.RelativeRect(new(0.025f + 0.25f * pos, 0.1f, 0.2f, 0.8f));
        if (pos < 4) {
            yield return new AddStageElement(this, rect).OnClick(() => {
                stages.Add(new());
                OnStageSelected?.Invoke(stages.Count - 1);
            });
        }
    }
}