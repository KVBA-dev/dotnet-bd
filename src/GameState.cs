using Raylib_cs;
using Game.Screens;
namespace Game;

public class GameState {
    public bool running;
    public Camera2D camera;
    public IScreen? currentScreen;
}