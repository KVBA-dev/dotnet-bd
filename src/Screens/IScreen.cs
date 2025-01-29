namespace Game.Screens;

public interface IScreen {
    public GameState State { get; init; }
    public Stack<ISubScreen> screens { get; init; }
    public void Update();
    public void Render();
}
