using Raylib_cs;
using Game.UI;

namespace Game.Screens;

public class EditorSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public Action OnBack { get; set; }
    public GameState State { get; init; }
    private List<UIElement> elements { get; init; }
    public FileExplorer fileExplorer { get; init; }

    Button btn_back;

    Label lbl_title;
    Label lbl_path;

    public UIElement FocusedElement => elements.Single(e => e.Focused);

    private void OpenEditor() {
        fileExplorer.OnLevelOpen -= OpenEditor;
        EditorScreen editor = new(State, fileExplorer.SelectedLevel);

        State.currentScreen = editor;
    }
    public EditorSubscreen(IScreen parent, GameState state) {
        Parent = parent;
        State = state;
        /* TODO:
           buttons to implement:
            - New Level
            - Publish Level
            - Remove Level
        */
        lbl_title = new(this, new(), "EDITOR");
        lbl_title.TextSize = 30;

        lbl_path = new(this, new(), "");

        btn_back = new Button(this, new(), "Back").OnClicked(() => OnBack?.Invoke());

        fileExplorer = new(this, new(), @"/editor");
        fileExplorer.OnLevelOpen += OpenEditor;

        OnBack = () => { };
        elements = [
            btn_back,
            fileExplorer,
            lbl_title,
            lbl_path,
        ];
        btn_back.Focused = true;
    }

    public void Render() {
        int titletextSize = (int)(30 * UISpecs.Scale);

        fileExplorer.Rect = new(UISpecs.Width / 10, UISpecs.Height / 5, UISpecs.Width * .8f, UISpecs.Height * .6f);
        if (!fileExplorer.Initialised) {
            fileExplorer.InitCanvas();
        }

        rl.BeginDrawing();
        rl.ClearBackground(Color.DarkBlue);

        foreach (UIElement e in elements) {
            e.Render();
        }
        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
        FocusedElement.Focused = false;
        element.Focused = true;
    }

    public void Update() {
        btn_back.Rect = UISpecs.ScreenRect.RelativeRect(.45f, .9f, .1f, .05f);
        lbl_title.Rect = UISpecs.ScreenRect.RelativeRect(0, 0, 1, .2f);
        fileExplorer.Rect = UISpecs.ScreenRect.RelativeRect(.1f, .2f, .8f, .6f);
        foreach (UIElement e in elements) {
            e.Update();
        }
    }
}

