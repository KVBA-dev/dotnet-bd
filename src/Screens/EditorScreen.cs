using Game.UI;
using Raylib_cs;

namespace Game.Screens;

public sealed class EditorScreen : IScreen, IUIHandler {
    private string levelPath = string.Empty;
    public GameState State { get; init; }
    public Stack<ISubScreen> screens { get; init; }
    public UIElement FocusedElement => throw new NotImplementedException();

    public EditorScreen(GameState state) {
        State = state;
        screens = new();
    }

    public void Render() {
        if (screens.Count > 0) {
            screens.Peek().Render();
            return;
        }
        (int width, int height) = (rl.GetScreenWidth(), rl.GetScreenHeight());
        rl.BeginDrawing();

        rl.ClearBackground(Color.Beige);

        rl.DrawText(levelPath, (int)(10 * Constants.UIScale), (int)(10 * Constants.UIScale), (int)(15 * Constants.UIScale), Color.Black);

        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
        throw new NotImplementedException();
    }

    public void LoadLevel(string path) {
        levelPath = path;
    }

    public void Update() {
        if (Input.UICancel) {
            MainMenuScreen mainMenu = new(State);
            State.currentScreen = mainMenu;
            GC.Collect();
            return;
        }
    }
}