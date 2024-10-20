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
        EditorScreen editor = new(State);
        editor.LoadLevel(fileExplorer.SelectedLevel);

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
        (int width, int height) = (rl.GetScreenWidth(), rl.GetScreenHeight());
        int titletextSize = (int)(30 * Constants.UIScale);

        int textWidth = rl.MeasureText("EDITOR", titletextSize);

        fileExplorer.Rect = new(width / 10, height / 5, width * .8f, height * .6f);
        if (!fileExplorer.Initialised) {
            fileExplorer.InitCanvas();
        }

        rl.BeginDrawing();

        rl.ClearBackground(Color.DarkBlue);

        rl.DrawText("EDITOR", (width - textWidth) / 2, height / 8, titletextSize, Color.White);

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

