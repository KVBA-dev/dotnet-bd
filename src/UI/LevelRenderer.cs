using Raylib_cs;
using Game.Levels;
using Game.Resources;
using System.Numerics;
using Game.Entities;
namespace Game.UI;

public sealed class LevelRenderer {
    public const int CHUNK_SIZE = Chunk.CHUNK_SIZE * Constants.TILE_SIZE;
    public Stage stage { get; set; }
    private Rectangle cameraRect;
    private readonly List<Chunk> chunks = new();
    private bool editorMode;
    public LevelRenderer(Stage stage, bool editorMode = false) {
        this.stage = stage;
        this.editorMode = editorMode;
    }

    private void GetCameraRect(ref Camera2D camera) {
        cameraRect.Position = rl.GetScreenToWorld2D(UISpecs.ScreenRect.Position, camera);
        cameraRect.Size = rl.GetScreenToWorld2D(new(UISpecs.Width, UISpecs.Height), camera) - cameraRect.Position;
    }

    private static Rectangle ChunkRect(Chunk c) => new(c.chunkX * CHUNK_SIZE, c.chunkY * CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE);

    private void CullChunks() {
        chunks.Clear();
        foreach (Chunk chunk in stage.chunks) {
            Rectangle chunkRect = ChunkRect(chunk);
            if (rl.CheckCollisionRecs(chunkRect, cameraRect)) {
                chunks.Add(chunk);
            }
        }
    }

    private void RenderChunk(Chunk chunk) {
        Vector2 chunkOrigin = new(chunk.chunkX * CHUNK_SIZE, chunk.chunkY * CHUNK_SIZE);
        //TODO
        Texture2D tex;

        for (int x = 0; x < Chunk.CHUNK_SIZE; x++) {
            for (int y = 0; y < Chunk.CHUNK_SIZE; y++) {
                if (chunk.tiles[x, y] == TileType.Empty) {
                    continue;
                }
                //TODO
                try {
                    tex = TextureRegistry.Tiles[chunk.tiles[x, y]];
                }
                catch {
                    tex = default;
                }
                rl.DrawTexturePro(tex, tex.Rectangle(), new(x * Constants.TILE_SIZE, y * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE), -chunkOrigin, 0, Color.White);
            }
        }
        if (editorMode) {
            rl.DrawRectangleLines((int)chunkOrigin.X, (int)chunkOrigin.Y, CHUNK_SIZE, CHUNK_SIZE, Color.Yellow);
        }
    }

    public void Render(ref Camera2D camera) {
        GetCameraRect(ref camera);
        CullChunks();
        for(int i = 0; i < chunks.Count; i++) {
            RenderChunk(chunks[i]);
        }
        Rectangle targetRect;
        if (stage.hasStart) {
            targetRect = new(stage.start * Constants.TILE_SIZE, new(Constants.TILE_SIZE, Constants.TILE_SIZE));
            rl.DrawTexturePro(TextureRegistry.Reg.Start, TextureRegistry.Reg.Start.Rectangle(), targetRect, Vector2.Zero, 0, Color.White);
        }
        if (stage.hasEnd) {
            targetRect = new(stage.end * Constants.TILE_SIZE, new(Constants.TILE_SIZE, Constants.TILE_SIZE));
            rl.DrawTexturePro(TextureRegistry.Reg.End, TextureRegistry.Reg.End.Rectangle(), targetRect, Vector2.Zero, 0, Color.White);
        }

        foreach(Entity e in stage.entities) {
            e.Render();
        }

        if (!editorMode) {
            return;
        }
        RenderChunkGrid(20, rl.ColorAlpha(Color.Black, .5f));
    }

    private void RenderChunkGrid(int radius, Color color) {
        Vector2 vertStart = new(-radius * CHUNK_SIZE, -radius * CHUNK_SIZE);
        Vector2 horiStart = vertStart;

        Vector2 vertEnd = new(-radius * CHUNK_SIZE, radius * CHUNK_SIZE);
        Vector2 horiEnd = vertEnd;

        for (int i = -radius; i <= radius; i++) {
            (horiStart.X, horiStart.Y) = (vertStart.Y, vertStart.X);
            (horiEnd.X, horiEnd.Y) = (vertEnd.Y, vertEnd.X);

            rl.DrawLineEx(vertStart, vertEnd, 4, color);
            rl.DrawLineEx(horiStart, horiEnd, 4, color);

            vertStart.X += CHUNK_SIZE;
            vertEnd.X += CHUNK_SIZE;
        }
    }
}