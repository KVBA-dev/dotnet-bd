namespace Game.Screens;

public class MainMenuScreen : IScreen {
    public GameState State { get; init; }
    public Stack<ISubScreen> screens { get; init; }
    public MainMenuScreen(GameState state) {
        State = state;
        screens = new();
        screens.Push(new MainMenuSubscreen(this, state));
        GC.Collect();
    }
    public void Render() {
        screens.Peek().Render();
    }

    public void Update() {
        screens.Peek().Update();
    }

}
