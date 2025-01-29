using Game.UI;
using Game.Levels;
using Game.Entities;
using Raylib_cs;
using GameBE.Data;
using System.Net.Http.Json;

namespace Game.Screens;

public sealed class GameSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public Action OnBack { get; set; }
    public GameState State { get; init; }

    public readonly List<UIElement> elements;
    public UIElement FocusedElement => elements.Single(e => e.Focused);

    public LevelInfo? info;

    Level level;
    int stageIdx;
    Stage currentStage => level.stages[stageIdx];
    bool levelCompleted = false;
    bool paused = false;

    LevelRenderer renderer;

    Label lbl_pause;
    Button btn_restart;
    Button btn_quit;

    Label lbl_stageName;
    float stageNameAlpha;

    Label lbl_attempts;
    float attemptsColor;

    int attempts = 1;

    public GameSubscreen(IScreen parent, GameState state, Level level) {
        Assert.That(level.stages.Count > 0);

        Parent = parent;
        State = state;
        State.camera.Zoom = 2;
        OnBack = () => { };

        this.level = level;
        stageIdx = 0;

        renderer = new(currentStage, false);
        lbl_stageName = new(this, new(), string.Empty);
        lbl_stageName.TextSize = 30;

        lbl_attempts = new(this, new(), "ATTEMPT 0");
        lbl_attempts.Alignment = Alignment.BottomLeft;
        InitStage();

        lbl_pause = new(this, new(), "PAUSED");
        lbl_pause.TextSize = 30;

        btn_restart = new Button(this, new(), "Restart").OnClicked(() => {
            if (levelCompleted) {
                attempts = 1;
                stageIdx = 0;
                renderer.stage = currentStage;
                levelCompleted = false;
                InitStage();
                return;
            }
            attempts += 1;
            attemptsColor = 1;
            InitStage();
            paused = false;
        });
        btn_restart.palette = ColorPalette.Editor;

        btn_quit = new Button(this, new(), "Quit").OnClicked(() => {
            OnBack?.Invoke();
        });
        btn_quit.palette = ColorPalette.Editor;


        elements = [
            lbl_pause,
            btn_quit,
            btn_restart,
        ];
    }

    private void InitStage() {
        currentStage.Init(State);
        currentStage.GetPlayer().OnDeath = (p) => {
            attempts += 1;
            attemptsColor = 1;
            currentStage.ResetEntities();
        };
        currentStage.GetPlayer().OnWin = _ => {
            levelCompleted = stageIdx == level.stages.Count - 1;
            if (levelCompleted) {
                currentStage.GetPlayer().controller.Enabled = false;
                if (info is null) {
                    return;
                }
                // FIXME:
                // Task.Run(CompleteLevel);
                return;
            }
            stageIdx += 1;
            renderer.stage = currentStage;
            InitStage();
        };
        lbl_stageName.Caption = currentStage.name;
        stageNameAlpha = 1;
    }

    public void Render() {
        rl.BeginDrawing();
        rl.ClearBackground(Color.Beige);
        rl.BeginMode2D(State.camera);
        {
            renderer.Render(ref State.camera);
        }
        rl.EndMode2D();

        lbl_stageName.Color = rl.ColorAlpha(Color.White, stageNameAlpha);
        lbl_stageName.Rect = UISpecs.ScreenRect.RelativeRect(0, 0, 1, .3f);

        lbl_attempts.Color = ColorLerp(Color.White, Color.Red, attemptsColor);
        lbl_attempts.Rect = UISpecs.ScreenRect.Padding(5);
        lbl_attempts.Caption = $"ATTEMPT {attempts}";
        if (levelCompleted) {
            rl.DrawRectangleRec(UISpecs.ScreenRect, rl.ColorAlpha(Color.Black, 0.5f));

            lbl_stageName.Caption = "Level completed!";

            lbl_attempts.Caption = $"Total attempts: {attempts}";
            lbl_attempts.Rect = UISpecs.ScreenRect.RelativeRect(0, .3f, 1, .1f);
        }

        lbl_stageName.Render();
        lbl_attempts.Render();
        attemptsColor -= Time.DeltaTime;

        if (paused || levelCompleted) {
            rl.DrawRectangleRec(UISpecs.ScreenRect, rl.ColorAlpha(Color.Black, 0.5f));
            foreach (UIElement e in elements) {
                e.Render();
            }
        }
        else {
            stageNameAlpha -= Time.DeltaTime * 0.2f;
        }

        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
        FocusedElement.Focused = false;
        element.Focused = true;
    }

    public void Update() {
        State.camera.Offset = UISpecs.ScreenRect.Size / 2;
        if (!levelCompleted) {
            paused ^= rl.IsKeyPressed(KeyboardKey.Escape);
        }
        if (paused || levelCompleted) {
            rl.ShowCursor();
            lbl_pause.Rect = UISpecs.ScreenRect.RelativeRect(0, 0, 1, 0.4f);
            btn_restart.Rect = UISpecs.ScreenRect.RelativeRect(0.45f, .5f, .1f, .05f);
            btn_quit.Rect = UISpecs.ScreenRect.RelativeRect(0.45f, .6f, .1f, .05f);

            foreach (UIElement e in elements) {
                e.Update();
            }
        }
        else {
            rl.HideCursor();
        }
        if (paused) {
            return;
        }
        foreach (Entity e in currentStage.entities) {
            e.Update();
        }
        while (currentStage.deathActions.Count > 0) {
            (Entity e, Action<Entity> action) = currentStage.deathActions.Dequeue();
            action?.Invoke(e);
        }
        State.camera.Target = Constants.Lerp(State.camera.Target, currentStage.GetPlayer().Pos * Constants.TILE_SIZE, Time.DeltaTime * 6);
    }

    private static Color ColorLerp(Color from, Color to, float t) {
        if (t < 0) {
            t = 0;
        }
        if (t > 1) {
            t = 1;
        }
        return new Color(
            (int)(from.R + t * (to.R - from.R)),
            (int)(from.G + t * (to.G - from.G)),
            (int)(from.B + t * (to.B - from.B)),
            (int)(from.A + t * (to.A - from.A))
        );
    }

    private async Task CompleteLevel() {
        if (State.loggedUser is null || info is null) {
            return;
        }

        using HttpClient client = new();

        HttpRequestMessage req = new(HttpMethod.Post, OnlineScreen.GetRoute("complete"));

        req.Content = JsonContent.Create(new LevelCompletionInfo() {
            levelid = info.levelid,
            playerid = State.loggedUser.userId,
            attempts = attempts,
        });

        HttpResponseMessage res = await client.SendAsync(req);
    }
}
