global using rl = Raylib_cs.Raylib;
using Raylib_cs;

using Game.Screens;
using Game.Resources;

namespace Game;

public class Game {
    static void Main() {
        GameState state = new();
        Init(state);
        GameLoop(state);
        Cleanup(state);
    }

    static void Init(GameState state) {
        rl.InitWindow(0, 0, "title");
        rl.SetWindowState(ConfigFlags.MaximizedWindow | ConfigFlags.BorderlessWindowMode);
        rl.SetExitKey(KeyboardKey.Null);
        rl.SetTargetFPS(60);

        state.running = true;
        state.currentScreen = new MainMenuScreen(state);

        IconRegistry.Load();
    }

    static void GameLoop(GameState state) {
        while (state.running && !rl.WindowShouldClose()) {
            Input.UpdateAxes(Settings.MainMap);
            if (state.currentScreen is null) {
                continue;
            }
            state.currentScreen.Update();
            state.currentScreen.Render();
        }
    }

    static void Cleanup(GameState state) {
        IconRegistry.Unload();

        rl.CloseWindow();
    }
}