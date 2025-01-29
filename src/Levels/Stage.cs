using System.Numerics;
using Game.Entities;
namespace Game.Levels;

public class Stage {
    public bool hasStart { get; set; }
    public bool hasEnd { get; set; }
    public Vector2 start { get; set; }
    public Vector2 end { get; set; }
    public string name { get; set; } = string.Empty;
    public List<Chunk> chunks { get; set; } = new();
    public readonly List<Entity> entities = new();

    // not a property coz it would mess up JSON serialisation
    private Player player;
    public Player GetPlayer() => player;
    public void SetPlayer(Player player) => this.player = player;
    //feels a bit cursed
    public readonly Queue<(Entity, Action<Entity>)> deathActions = new();

    public static bool operator == (Stage lhs, Stage rhs) {
        Assert.That(lhs.chunks.Count == rhs.chunks.Count);
        bool equalChunks = true;
        for (int i = 0; i < lhs.chunks.Count; i++) {
            equalChunks &= lhs.chunks[i] == rhs.chunks[i];
        }
        return lhs.start == rhs.start && lhs.end == rhs.end && lhs.hasStart == rhs.hasStart && lhs.hasEnd == rhs.hasEnd
            && equalChunks;
    }

    public static bool operator != (Stage lhs, Stage rhs) {
        return !(lhs == rhs);
    }

    public Stage Clone() {
        return new() {
            start = start,
            end = end,
            hasStart = hasStart,
            hasEnd = hasEnd,
            name = name,
            chunks = chunks.Select(c => c.Clone()).ToList(),
        };
    }

    public void CopyFrom(Stage other) {
        start = other.start;
        end = other.end;
        hasStart = other.hasStart;
        hasEnd = other.hasEnd;
        name = other.name;
        chunks = other.chunks.Select(c => c.Clone()).ToList();
    }

    public TileType GetTileAt(int x, int y) {
        int chunkX = (int)Math.Floor((float)x / Chunk.CHUNK_SIZE);
        int chunkY = (int)Math.Floor((float)y / Chunk.CHUNK_SIZE);

        if (!chunks.Any(c => c.chunkX == chunkX && c.chunkY == chunkY)) {
            return TileType.Empty;
        }

        int tileX = x - chunkX * Chunk.CHUNK_SIZE;
        int tileY = y - chunkY * Chunk.CHUNK_SIZE;

        return chunks.Single(c => c.chunkX == chunkX && c.chunkY == chunkY).tiles[tileX, tileY];
    }
    
    public byte GetCollisionTileAt(int x, int y) {
        int chunkX = (int)Math.Floor((float)x / Chunk.CHUNK_SIZE);
        int chunkY = (int)Math.Floor((float)y / Chunk.CHUNK_SIZE);

        if (!chunks.Any(c => c.chunkX == chunkX && c.chunkY == chunkY)) {
            return 0;
        }

        int tileX = x - chunkX * Chunk.CHUNK_SIZE;
        int tileY = y - chunkY * Chunk.CHUNK_SIZE;

        return chunks.Single(c => c.chunkX == chunkX && c.chunkY == chunkY).collisionMask[tileX, tileY];
    }

    public void SetCollisionTileAt(int x, int y, byte mask) {
        int chunkX = (int)Math.Floor((float)x / Chunk.CHUNK_SIZE);
        int chunkY = (int)Math.Floor((float)y / Chunk.CHUNK_SIZE);

        if (!chunks.Any(c => c.chunkX == chunkX && c.chunkY == chunkY)) {
            return;
        }

        int tileX = x - chunkX * Chunk.CHUNK_SIZE;
        int tileY = y - chunkY * Chunk.CHUNK_SIZE;

        chunks.Single(c => c.chunkX == chunkX && c.chunkY == chunkY).collisionMask[tileX, tileY] = mask;
    }

    public void Init(GameState state) {
        deathActions.Clear();
        foreach (Chunk c in chunks) {
            c.PrepareCollisionMask();
        }
        player = new(state, this);
        ResetEntities();
    }

    public void ResetEntities() {
        entities.Clear();
        entities.Add(player);
        player.GoTo(start - new Vector2(0, 0.1f));
    }

    public void Quit() {
        entities.Clear();
    }
}