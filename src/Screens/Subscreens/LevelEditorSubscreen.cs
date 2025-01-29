using Raylib_cs;
using Game.UI;
using Game.Levels;
using Game.Resources;
using GameBE.Data;
using System.Net.Http.Json;

namespace Game.Screens;

public sealed class LevelEditorSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public Action OnBack { get; set; }
    public GameState State { get; init; }
    private Level? level => (Parent as EditorScreen)?.Level;

    public UIElement FocusedElement => elements.Single(e => e.Focused);
    private InputField inpName;
    private InputField inpAuthor;
    private Button btnBack;
    private ImageButton btnPublish;
    private ImageButton btnPlay;
    private StageSelector selector;
    private List<UIElement> elements;
    private LoadingElement loading;

    private Task<bool>? currentTask;

    public LevelEditorSubscreen(IScreen parent, GameState state) {
        Parent = parent;
        State = state;

        currentTask = null;

        if (parent is not EditorScreen) {
            return;
        }

        OnBack = () => { };

        inpName = new(this, new());
        inpName.OnEditingEnd += (name) => {
            if (level is null) {
                return;
            }
            level.name = name;
        };

        inpAuthor = new(this, new());
        inpAuthor.OnEditingEnd += (author) => {
            if (level is null) {
                return;
            }
            level.author = author;
        };

        btnBack = new(this, new(), "Back");
        btnBack.OnClick += (parent as EditorScreen).ExitEditor;

        btnPlay = new(this, new(), IconRegistry.Reg.EditorPlay);
        btnPlay.OnClick += () => {
            GameSubscreen game = new(parent, state, level);
            game.OnBack = () => { parent.screens.Pop(); };
            parent.screens.Push(game);
        };

        loading = new(this, new());

        if (state.loggedUser is not null) {

            btnPublish = new ImageButton(this, new(), IconRegistry.Reg.EditorPublish);
            btnPublish.OnClick += () => {
                if (currentTask is not null) {
                    return;
                }

                currentTask = Task.Run(PublishLevel);
            };
        }

        selector = new(this, new(), level);
        selector.OnStageSelected += (idx) => {
            // open stage editor
            OpenStageEditor(level.stages[idx]);
        };

        loading = new(this, new());

        elements = new() {
            inpName,
            inpAuthor,
            selector,
            btnPlay,
            btnBack,
            loading,
        };
        if (state.loggedUser is not null) {
            elements.Add(btnPublish);
        }
        UpdateUI();

        // (parent as EditorScreen).OnLevelLoaded += UpdateUI;
    }

    public void UpdateUI() {
        inpName.Text = level.name;
        inpAuthor.Text = level.author;
    }

    public void Render() {
        rl.BeginDrawing();
        rl.ClearBackground(Color.DarkBlue);

        rl.DrawText("NAME", (int)(10 * UISpecs.Scale), (int)(10 * UISpecs.Scale), (int)(15 * UISpecs.Scale), Color.White);
        rl.DrawText("AUTHOR", (int)(10 * UISpecs.Scale), (int)(35 * UISpecs.Scale), (int)(15 * UISpecs.Scale), Color.White);

        foreach (UIElement element in elements) {
            element.Render();
        }

        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
        if (elements.Any(e => e.Focused)) {
            FocusedElement.Focused = false;
        }
        element.Focused = true;
    }

    public void OpenStageEditor(Stage stage) {
        StageEditorSubscreen stageEditor = new(Parent, State, stage);
        stageEditor.OnBack = () => Parent.screens.Pop();
        Parent.screens.Push(stageEditor);
    }

    public void Update() {
        Rectangle nameRect = UISpecs.ScreenRect.RelativeRect(new(0.1f, 0, 0.8f, 0.05f));
        nameRect.Y = (int)(10 * UISpecs.Scale);
        inpName.Rect = nameRect;
        nameRect.Y = (int)(35 * UISpecs.Scale);
        inpAuthor.Rect = nameRect;

        Rectangle selectorRect = UISpecs.ScreenRect.RelativeRect(new(0.1f, 0.2f, 0.8f, 0.3f));
        selector.Rect = selectorRect;

        btnBack.Rect = new(10 * UISpecs.Scale, UISpecs.Height - 32.5f * UISpecs.Scale, 100 * UISpecs.Scale, 22.5f * UISpecs.Scale);

        btnPlay.Rect = UISpecs.ScreenRect.RelativeRect(0.4f, 0.65f, 0.2f, 0.2f).CenteredSquare();
        btnPlay.ImageRect = btnPlay.Rect.RelativeRect(0.1f, 0.1f, 0.8f, 0.8f);
        if (State.loggedUser is not null) {
            btnPublish.Rect = UISpecs.ScreenRect.RelativeRect(0.4f, 0.85f, 0.2f, 0.1f).CenteredSquare();
            btnPublish.ImageRect = btnPublish.Rect.RelativeRect(0.1f, 0.1f, 0.8f, 0.8f);
        }

        if (currentTask is not null) {
            if (!currentTask.IsCompleted) {
                return;
            }
            if (currentTask.Result) {

            }
            currentTask = null;
            return;
        }
        foreach (UIElement element in elements) {
            element.Update();
        }
    }

    public async Task<bool> PublishLevel() {
        HttpClient client = new();
        HttpRequestMessage req = new(HttpMethod.Post, OnlineScreen.GetRoute("level"));

        LevelCreationInfo info = new();
        info.creatorid = State.loggedUser.userId;
        info.levelname = level.name;
        string? levelStr = await LevelLoader.LevelToString(level);
        if (levelStr is null) {
            return false;
        }
        Assert.That(level.stages.All(s => s.start != s.end));
        info.level = (string)levelStr;

        req.Content = JsonContent.Create(info);
        HttpResponseMessage res = await client.SendAsync(req);
        return res.IsSuccessStatusCode;
    }
}
