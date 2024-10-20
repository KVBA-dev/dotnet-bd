using Raylib_cs;
using Game.UI;

namespace Game.Screens;

public class MainMenuSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public GameState State { get; init; }
    public List<UIElement> Elements { get; init; }
    public UIElement FocusedElement => Elements.Where(e => e.Focused).Single();
    public Action OnBack { get; set; }
    int selected = 0;
    public MainMenuSubscreen(IScreen parent, GameState state) {
        Parent = parent;
        State = state;
        Elements = new()
        {
            new Button(this, new(), "Play"),

            new Button(this, new(), "Browse"),

            new Button(this, new(), "Editor").OnClicked(() => {
                EditorSubscreen editor = new(parent, state);
                editor.OnBack = () => parent.screens.Pop();
                parent.screens.Push(editor);
            }),

            new Button(this, new(), "Settings").OnClicked(() => {
                SettingsMenuSubscreen settings = new(parent, state);
                settings.OnBack = () => parent.screens.Pop();
                parent.screens.Push(settings); 
            }),

            new Button(this, new(), "Quit").OnClicked(() => state.running = false),
        };
        Elements.Single(e => (e as Button)?.Caption == "Play").Focused = true;
        OnBack = () => {};
    }

    public void SetFocused(UIElement element) {
        FocusedElement.Focused = false;
        element.Focused = true;
        selected = Elements.IndexOf(element);
    }

    public void Render() {
        (int width, int height) = (rl.GetScreenWidth(), rl.GetScreenHeight());
        int titleTextSize = (int)(30 * Constants.UIScale);
        int optionTextSize = (int)(15 * Constants.UIScale);

        Rectangle box = new();
        int textWidth = rl.MeasureText("TRIAL MAKER", titleTextSize);
        int textY;

        rl.BeginDrawing();
        rl.ClearBackground(Color.DarkBlue);

        rl.DrawText("TRIAL MAKER", (width - textWidth) / 2, height / 8, titleTextSize, Color.White);
        int i = 0;
        foreach (Button button in Elements) {
            textY = height / 2 + 2 * i * optionTextSize;

            box.Width = 100 * Constants.UIScale;
            box.Height = Constants.UIScale * 15 * 3 / 2;
            box.X = (width - box.Width) / 2;
            box.Y = textY - box.Height / 6;

            button.Rect = box;
            button.Render();

            i++;
        }

        rl.EndDrawing();
    }

    public void Update() {
        if (Input.UIDown) {
            selected = ++selected % Elements.Count;
        }
        if (Input.UIUp) {
            selected--;
            if (selected < 0) {
                selected += Elements.Count;
            }
        }
        SetFocused(Elements[selected]);

        foreach (UIElement element in Elements) {
            element.Update();
        }

        if (Input.UIConfirm) {
            UIElement element = FocusedElement;
            element.OnUIConfirm?.Invoke(element);
        }
        if (Input.UICancel) {
            OnBack?.Invoke();
        }
    }

}