using Game;
using Game.Screens;

public sealed class OnlineScreen : IScreen {
    public GameState State { get; init; }
    public Stack<ISubScreen> screens { get; init; }

    public static readonly Uri URL = new("http://localhost:8000");

    public static string GetRoute(string route) => $"{URL.AbsoluteUri}{route}";

    public OnlineScreen(GameState state) {
        State = state;
        screens = new();
        ISubScreen firstSubscreen;
        if (state.loggedUser is null) {
            firstSubscreen = new LoginSubscreen(this, state);
        }
        else {
            firstSubscreen = new LevelBrowserSubscreen(this, state);
        }

        firstSubscreen.OnBack += () => {
            MainMenuScreen scr = new(state);
            state.currentScreen = scr;
        };

        screens.Push(firstSubscreen);
        GC.Collect();

    }

    public void Render() {
        screens.Peek().Render();
    }

    public void Update() {
        screens.Peek().Update();
    }
}
