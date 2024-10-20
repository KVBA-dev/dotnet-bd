using Game.UI;
using Raylib_cs;

namespace Game.Screens;

public class SettingsMenuSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public GameState State { get; init; }
    public List<UIElement> Elements { get; init; }
    public List<UIElement> PassiveElements { get; init; }
    public IEnumerable<UIElement> AllElements => Elements.Concat(PassiveElements);
    public UIElement FocusedElement => Elements.Where(e => e.Focused).Single();
    public Action OnBack { get; set; }

    Label lblVolume;
    Button btnBack;
    public SettingsMenuSubscreen(IScreen parent, GameState state) {
        Parent = parent;
        State = state;

        lblVolume = new(this, new(), "Volume") {
            Alignment = Alignment.CenterRight
        };

        btnBack = new Button(this, new(), "Back").OnClicked(() => OnBack?.Invoke());
        btnBack.Focused = true;

        Elements = new() {
            btnBack,
        };

        PassiveElements = new() {
            lblVolume,
        };
    }

    public void Render() {
        (int width, int height) = (rl.GetScreenWidth(), rl.GetScreenHeight());

        lblVolume.Rect = new(200 * Constants.UIScale, height / 2, 200 * Constants.UIScale, 30);
        btnBack.Rect = new(width / 2 - 50 * Constants.UIScale, height - 50 * Constants.UIScale, 100 * Constants.UIScale, 30 * Constants.UIScale);

        rl.BeginDrawing();
        rl.ClearBackground(Color.DarkBlue);
        foreach (UIElement element in AllElements) {
            element.Render();
        }
        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
    }

    public void Update() {
        foreach (UIElement element in Elements) {
            element.Update();
        }
        if (Input.UIConfirm) {
            FocusedElement.OnUIConfirm(FocusedElement);
        }
        if (Input.UICancel) {
            OnBack?.Invoke();
        }
    }
}