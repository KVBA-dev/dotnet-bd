using System.Data;
using Game.Levels;
using Game.UI;
using Raylib_cs;

namespace Game.Screens;

public sealed class StageEditorSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public Action OnBack { get; set; } = () => { };
    public GameState State { get; init; }

    public UIElement FocusedElement => throw new NotImplementedException();

    public StageEditorSubscreen(IScreen parent, GameState state, Stage stage) {
        State = state;
        Parent = parent;
    }

    public void Render() {
        rl.BeginDrawing();
        rl.ClearBackground(Color.Beige);
        rl.BeginMode2D(State.camera); {

        }
        rl.EndMode2D();
        // UI goes here

        // --------
        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
        throw new NotImplementedException();
    }

    public void Update() {
        if (rl.IsKeyPressed(KeyboardKey.Escape)) {
            OnBack?.Invoke();
        }
    }
}