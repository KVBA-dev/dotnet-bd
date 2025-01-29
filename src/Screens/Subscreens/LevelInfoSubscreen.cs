using Game.UI;
using Game.Levels;
using Game.Resources;
using GameBE.Data;
using System.Text.Json;
using System.Net.Http.Json;
using Raylib_cs;

namespace Game.Screens;

public sealed class LevelInfoSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public Action OnBack { get; set; }
    public GameState State { get; init; }

    Label lbl_name;
    Label lbl_author;
    ImageButton btn_play;
    ImageButton btn_comments;
    ImageButton btn_like;
    Button btn_back;

    public readonly List<UIElement> elements;
    public UIElement FocusedElement => elements.Single(e => e.Focused);

    public LevelInfo Info;
    private Level level;

    bool liked;
    bool completed;
    int attempts;

    Task? currentTask = null;

    public LevelInfoSubscreen(IScreen parent, GameState state, LevelInfo info) {
        Parent = parent;
        State = state;
        Info = info;

        OnBack = () => { };

        lbl_name = new(this, new(), Info.levelname);
        lbl_name.TextSize = 35;
        lbl_author = new(this, new(), Info.creatorname);
        lbl_author.TextSize = 20;

        btn_back = new Button(this, new(), "Back").OnClicked(() => {
            OnBack?.Invoke();
        });
        btn_play = new(this, new(), IconRegistry.Reg.EditorPlay);
        btn_play.OnClick += () => {
            if (currentTask is not null) {
                return;
            }
            currentTask = Task.Run(PlayLevel);
        };
        btn_like = new(this, new(), IconRegistry.Reg.Like);
        btn_like.OnClick += () => {
            liked ^= true;
            Task.Run(ToggleLike);
        };

        btn_comments = new(this, new(), IconRegistry.Reg.Comment);
        btn_comments.OnClick += () => {
            // TODO: comments screen
        };

        elements = [
            lbl_name,
            lbl_author,
            btn_play,
            btn_back,
            btn_like,
            btn_comments,
        ];
    }

    public void Render() {
        rl.BeginDrawing();
        rl.ClearBackground(ColorPalette.Default.background);
        btn_like.Tint = liked ? Color.Red : Color.White;
        foreach (UIElement e in elements) {
            e.Render();
        }
        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
        FocusedElement.Focused = false;
        element.Focused = true;
    }

    public void Update() {
        lbl_name.Rect = UISpecs.ScreenRect.RelativeRect(0, 0, 1, 0.15f);
        lbl_author.Rect = UISpecs.ScreenRect.RelativeRect(0, 0.15f, 1, 0.1f);
        btn_play.Rect = UISpecs.ScreenRect.CenteredSquare(.8f);
        btn_play.ImageRect = btn_play.Rect.CenteredSquare(.1f);
        btn_like.Rect = UISpecs.ScreenRect.RelativeRect(.5f, 0, .5f, 1).CenteredSquare(.75f);
        btn_like.ImageRect = btn_like.Rect.CenteredSquare(.1f);
        btn_comments.Rect = UISpecs.ScreenRect.RelativeRect(0, 0, .5f, 1).CenteredSquare(.75f);
        btn_comments.ImageRect = btn_comments.Rect.CenteredSquare(.1f);
        btn_back.Rect = UISpecs.ScreenRect.RelativeRect(.45f, .9f, .1f, .05f);

        foreach (UIElement e in elements) {
            e.Update();
        }
    }

    public async Task PlayLevel() {
        string levelPath = Path.Join(Environment.CurrentDirectory, "editor");
        levelPath = Path.Join(levelPath, $"{Info.levelid}.tmv");

        if (File.Exists(levelPath)) {
            Level? level = LevelLoader.LoadLevel(levelPath);
            if (level is null) {
                return;
            }
            this.level = level;
            StartLevel();
            return;
        }

        await DownloadLevel(levelPath);
    }

    public async Task DownloadLevel(string targetPath) {
        using HttpClient client = new();
        HttpRequestMessage req = new(HttpMethod.Get, OnlineScreen.GetRoute($"level/{Info.levelid}"));

        HttpResponseMessage res = await client.SendAsync(req);

        if (!res.IsSuccessStatusCode) {
            return;
        }

        Level? level = LevelLoader.LevelFromString(await res.Content.ReadAsStringAsync());

        if (level is null || level.stages.Count == 0) {
            return;
        }
        this.level = level;
        LevelLoader.SaveLevel(targetPath, level);
        StartLevel();
    }

    public async Task FetchUserLevelInfo() {
        if (State.loggedUser is null) {
            return;
        }

        using HttpClient client = new();

        HttpRequestMessage req = new(HttpMethod.Post, OnlineScreen.GetRoute("levelinfo"));
        req.Content = JsonContent.Create(new LevelInfoRequest() {
            playerid = State.loggedUser.userId,
            levelid = Info.levelid,
        });

        HttpResponseMessage res = await client.SendAsync(req);

        LevelInfoResponse? infoRes = await JsonSerializer.DeserializeAsync<LevelInfoResponse>(await res.Content.ReadAsStreamAsync());

        if (infoRes is null) {
            return;
        }

        attempts = infoRes.attempts;
        completed = infoRes.completed;
        liked = infoRes.liked;
    }

    public async Task ToggleLike() {
        if (State.loggedUser is null) {
            return;
        }

        using HttpClient client = new();

        HttpRequestMessage req = new(HttpMethod.Post, OnlineScreen.GetRoute("like"));
        req.Content = JsonContent.Create(new LikeInfo() {
            levelid = Info.levelid,
            playerid = State.loggedUser.userId,
            like = liked,
        });

        await client.SendAsync(req);
    }

    private void StartLevel() {
        GameSubscreen game = new(Parent, State, level);
        game.info = Info;
        game.OnBack = () => { Parent.screens.Pop(); };
        Parent.screens.Push(game);
    }
}
