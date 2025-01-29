using Game.Levels;
using Game.UI;

namespace Game.Screens;

public sealed class EditorScreen : IScreen, IUIHandler {
    private string levelPath = string.Empty;
    public GameState State { get; init; }
    public Stack<ISubScreen> screens { get; init; }
    public UIElement FocusedElement => throw new NotImplementedException();
    private Level level;
    public Level Level => level;
    public event Action OnLevelLoaded;

    public EditorScreen(GameState state, string levelPath) {
        State = state;
        screens = new();
        level = new();
        LoadLevel(levelPath);
        screens.Push(new LevelEditorSubscreen(this, state));
        OnLevelLoaded = () => { };
        GC.Collect();
    }

    public void Render() {
        if (screens.Count > 0) {
            screens.Peek().Render();
        }
    }

    public void SetFocused(UIElement element) {
        throw new NotImplementedException();
    }

    public void LoadLevel(string path) {
        levelPath = path;
        Level? level = LevelLoader.LoadLevel(path);
        this.level = level ?? new();
        OnLevelLoaded?.Invoke();
    }

    public void ExitEditor() {
        LevelLoader.SaveLevel(levelPath, level);
        State.currentScreen = new MainMenuScreen(State);
    }

    public void Update() {
        if (screens.Count > 0) {
            screens.Peek().Update();
            return;
        }
        if (Input.UICancel) {
            MainMenuScreen mainMenu = new(State);
            State.currentScreen = mainMenu;
            GC.Collect();
            return;
        }
    }
}
