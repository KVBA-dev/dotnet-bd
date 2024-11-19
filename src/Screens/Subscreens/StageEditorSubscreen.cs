using Game.UI;
using Game.Levels;
using Game.Resources;
using Raylib_cs;
using System.Numerics;

namespace Game.Screens;

public sealed class StageEditorSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public Action OnBack { get; set; } = () => { };
    public GameState State { get; init; }
    private Stage originalStage;
    private readonly Stage editedStage;
    int selectedTile = 0;

    LevelRenderer renderer;
    // ui
    private bool mouseOnUI;
    private readonly InputField inp_name;
    private readonly ImageButton btn_exit;
    private readonly ImageButton btn_save;
    private readonly TileSelector tileSelector;

    private List<UIElement> elements = new();
    public UIElement FocusedElement => elements.Single(e => e.Focused);
    

    public StageEditorSubscreen(IScreen parent, GameState state, Stage stage) {
        State = state;
        Parent = parent;
        originalStage = stage;
        editedStage = originalStage.Clone();

        state.camera.Target = editedStage.start;

        inp_name = new(this, new());
        btn_exit = new(this, new(), IconRegistry.Reg.EditorExit);
        btn_save = new(this, new(), IconRegistry.Reg.EditorSave);
        tileSelector = new(this, new());

        renderer = new(editedStage, true);

        InitUI();
    }

    private void InitUI() {
        inp_name.OnEditingEnd += (name) => {
            editedStage.name = name;
        };
        inp_name.palette = ColorPalette.Editor;
        inp_name.Text = editedStage.name;

        btn_exit.OnClick += () => {
            OnBack?.Invoke();
        };
        btn_exit.palette = ColorPalette.Editor;

        btn_save.OnClick += () => {
            //purge empty chunks
            editedStage.chunks = editedStage.chunks.Where(c => c.tiles.Cast<TileType>().Sum((t) => (int)t) > 0).ToList();
            originalStage.CopyFrom(editedStage);
        };
        btn_save.palette = ColorPalette.Editor;

        tileSelector.OnTileSelected += (tile, button) => {
            selectedTile = (int)tile;
        };

        elements = [
            inp_name,
            btn_exit,
            btn_save,
            tileSelector,
        ];

    }

    public void AddElements(IEnumerable<UIElement> elements) {
        this.elements.AddRange(elements);
    }

    public void Render() {
        State.camera.Offset = UISpecs.ScreenRect.Size / 2;
        rl.BeginDrawing();
        rl.ClearBackground(Color.Beige);
        rl.BeginMode2D(State.camera); {
            renderer.Render(ref State.camera);
        }
        rl.EndMode2D();
        // UI goes here
        rl.DrawRectangleRec(tileSelector.GetSelectedRect(selectedTile), rl.ColorAlpha(Color.Green, .5f));
        foreach (UIElement e in elements) {
            e.Render();
        }
        // --------
        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
        if (elements.Any(e => e.Focused)) {
            FocusedElement.Focused = false;
        }
        element.Focused = true;
    }

    Vector2 cameraPos;
    Vector2 mousePos;
    Vector2 delta;
    bool dragging = false;
    public void Update() {
        mouseOnUI = false;
        if (rl.IsKeyPressed(KeyboardKey.Escape)) {
            OnBack?.Invoke();
            return;
        }
        Rectangle nameRect = UISpecs.ScreenRect.RelativeRect(.3f, 0, .6f, .05f);
        nameRect.Y = UISpecs.Scale * 10;
        inp_name.Rect = nameRect;

        Rectangle btnRect = new(new Vector2(20, 20) * UISpecs.Scale, new Vector2(30, 30) * UISpecs.Scale);
        btn_exit.Rect = btnRect;
        btn_exit.ImageRect = btn_exit.Rect.RelativeRect(.1f, .1f, .8f, .8f);

        btnRect.X += 40 * UISpecs.Scale;
        btn_save.Rect = btnRect;
        btn_save.ImageRect = btn_save.Rect.RelativeRect(.1f, .1f, .8f, .8f);

        btnRect = new(20 * UISpecs.Scale, UISpecs.Height - 140 * UISpecs.Scale, new Vector2(120, 120) * UISpecs.Scale);
        tileSelector.Rect = btnRect;

        foreach (UIElement e in elements) {
            mouseOnUI |= e.Update();
        }

        if (mouseOnUI) {
            return;
        }

        //try place blocks
        bool leftMouse = rl.IsMouseButtonPressed(MouseButton.Left)
                      || rl.IsKeyDown(KeyboardKey.LeftShift) && rl.IsMouseButtonDown(MouseButton.Left);

        if (leftMouse) {
            Vector2 point = rl.GetScreenToWorld2D(Input.MousePosition, State.camera);
            Vector2 chunkCoords = point / LevelRenderer.CHUNK_SIZE;
            int chunkX = (int)Math.Floor(chunkCoords.X);
            int chunkY = (int)Math.Floor(chunkCoords.Y);

            if (!editedStage.chunks.Any(c => c.chunkX == chunkX && c.chunkY == chunkY )) {
                Chunk newChunk = new() {
                    chunkX = chunkX,
                    chunkY = chunkY,
                };
                editedStage.chunks.Add(newChunk);
            }

            Chunk editedChunk = editedStage.chunks.Where(c => c.chunkX == chunkX && c.chunkY == chunkY).Single();

            Vector2 pointCoords = point / Constants.TILE_SIZE;
            int tileX = (int)Math.Floor(pointCoords.X) % Chunk.CHUNK_SIZE;
            int tileY = (int)Math.Floor(pointCoords.Y) % Chunk.CHUNK_SIZE;
            if (tileX < 0) {
                tileX += Chunk.CHUNK_SIZE;
            }
            if (tileY < 0) {
                tileY += Chunk.CHUNK_SIZE;
            }

            editedChunk.tiles[tileX, tileY] = (TileType)selectedTile;

            return;
        }

        float mouseDelta = rl.GetMouseWheelMove();
        State.camera.Zoom += mouseDelta / 4;
        if (State.camera.Zoom < .5f) {
            State.camera.Zoom = .5f;
        }
        if (State.camera.Zoom > 2) {
            State.camera.Zoom = 2;
        }

        // pan camera
        if (rl.IsMouseButtonDown(MouseButton.Right)) {
            if (!dragging) {
                mousePos = Input.MousePosition;
                dragging = true;
            }
            delta = (Input.MousePosition - mousePos) / State.camera.Zoom;
            State.camera.Target = cameraPos - delta;
        }
        else {
            if (dragging) {
                cameraPos -= delta;
            }
            State.camera.Target = cameraPos;
            dragging = false;
        }
    }
}