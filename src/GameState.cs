using Raylib_cs;
using Game.Screens;
using Game.User;
namespace Game;

public class GameState {
    public bool running;
    public Camera2D camera;
    public IScreen? currentScreen;
    public LoggedUser? loggedUser;
}
