using Raylib_cs;
using Game.UI;

namespace Game.Screens;

public class EditorSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public Action OnBack { get; set; }
    public GameState State { get; init; }
    private List<UIElement> elements { get; init; }
    public FileExplorer fileExplorer { get; init; }

    public UIElement FocusedElement {
        get {
            if (fileExplorer.Focused) {
                return fileExplorer;
            }
            return elements.Single(e => e.Focused);
        }
    } 

    private void OpenEditor() {
        fileExplorer.OnLevelOpen -= OpenEditor;
        EditorScreen editor = new(State, fileExplorer.SelectedLevel);
        // editor.LoadLevel(fileExplorer.SelectedLevel);

        State.currentScreen = editor;
        GC.Collect();
    }
    public EditorSubscreen(IScreen parent, GameState state) {
        Parent = parent;
        State = state;
        /*
        buttons to implement:
        - New Level
        - Publish Level
        - Remove Level
        - Back
        */
        elements = new();
        fileExplorer = new(this, new(), @"\editor");
        fileExplorer.OnLevelOpen += OpenEditor;
        OnBack = () => { };
        fileExplorer.Focused = true;
    }

    public void Render() {
        int titletextSize = (int)(30 * UISpecs.Scale);

        int textWidth = rl.MeasureText("EDITOR", titletextSize);

        fileExplorer.Rect = new(UISpecs.Width / 10, UISpecs.Height / 5, UISpecs.Width * .8f, UISpecs.Height * .6f);
        if (!fileExplorer.Initialised) {
            fileExplorer.InitCanvas();
        }

        rl.BeginDrawing();

        rl.ClearBackground(Color.DarkBlue);

        rl.DrawText("EDITOR", (UISpecs.Width - textWidth) / 2, UISpecs.Height / 8, titletextSize, Color.White);

        fileExplorer.Render();

        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
        FocusedElement.Focused = false;
        element.Focused = true;
    }

    public void Update() {
        if (rl.IsKeyPressed(KeyboardKey.Escape)) {
            OnBack?.Invoke();
            return;
        }
        FocusedElement.Update();
    }
}

