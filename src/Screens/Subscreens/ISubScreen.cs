namespace Game.Screens;

public interface ISubScreen {
    public IScreen Parent{ get; init; }
    public Action OnBack { get; set; }
    public GameState State { get; init; }
    public void Update();
    public void Render();
}
