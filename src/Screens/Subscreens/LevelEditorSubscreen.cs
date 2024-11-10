using Raylib_cs;
using Game.UI;
using Game.Levels;

namespace Game.Screens;

public sealed class LevelEditorSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public Action OnBack { get; set; }
    public GameState State { get; init; }
    private Level? level => (Parent as EditorScreen)?.Level;

    public UIElement FocusedElement => elements.Single(e => e.Focused);
    private InputField inpName;
    private InputField inpAuthor;
    private Button btnBack;
    private StageSelector selector;
    private List<UIElement> elements;
    public LevelEditorSubscreen(IScreen parent, GameState state) {
        Parent = parent;
        State = state;

        OnBack = () => { };

        inpName = new(this, new());
        inpName.OnEditingEnd += (name) => {
            if (level is null) {
                return;
            }
            level.name = name;
        };

        inpAuthor = new(this, new());
        inpAuthor.OnEditingEnd += (author) => {
            if (level is null) {
                return;
            }
            level.author = author;
        };

        btnBack = new(this, new(), "Back");
        btnBack.OnClick += (parent as EditorScreen).ExitEditor;

        selector = new(this, new(), level);
        selector.OnStageSelected += (idx) => {
            // open stage editor
            OpenStageEditor(level.stages[idx]);
        };

        elements = new() {
            inpName,
            inpAuthor,
            selector,
            btnBack,
        };
        UpdateUI();

        // (parent as EditorScreen).OnLevelLoaded += UpdateUI;
    }

    public void UpdateUI() {
        inpName.Text = level.name;
        inpAuthor.Text = level.author;
    }

    public void Render() {
        rl.BeginDrawing();
        rl.ClearBackground(Color.DarkBlue);

        rl.DrawText("NAME", (int)(10 * UISpecs.Scale), (int)(10 * UISpecs.Scale), (int)(15 * UISpecs.Scale), Color.White);
        rl.DrawText("AUTHOR", (int)(10 * UISpecs.Scale), (int)(35 * UISpecs.Scale), (int)(15 * UISpecs.Scale), Color.White);

        foreach(UIElement element in elements) {
            element.Render();
        }

        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
        if (elements.Any(e => e.Focused)) {
            FocusedElement.Focused = false;
        }
        element.Focused = true;
    }

    public void OpenStageEditor(Stage stage) {
        Console.WriteLine("Opening stage");
        StageEditorSubscreen stageEditor = new(Parent, State, stage);
        stageEditor.OnBack = () => Parent.screens.Pop();
        Parent.screens.Push(stageEditor);
    }

    public void Update() {
        Rectangle nameRect = UISpecs.ScreenRect.RelativeRect(new(0.1f, 0, 0.8f, 0.05f));
        nameRect.Y = (int)(10 * UISpecs.Scale);
        inpName.Rect = nameRect;
        nameRect.Y = (int)(35 * UISpecs.Scale);
        inpAuthor.Rect = nameRect;

        Rectangle selectorRect = UISpecs.ScreenRect.RelativeRect(new(0.1f, 0.2f, 0.8f, 0.3f));
        selector.Rect = selectorRect;

        btnBack.Rect = new(10 * UISpecs.Scale, UISpecs.Height - 32.5f * UISpecs.Scale, 100 * UISpecs.Scale, 22.5f * UISpecs.Scale);

        foreach (UIElement element in elements) {
            element.Update();
        }
    }
}