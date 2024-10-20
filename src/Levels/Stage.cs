using System.Numerics;
namespace Game.Levels;

public class Stage {
    public Vector2 start { get; set; }
    public Vector2 end { get; set; }
    public List<Chunk> chunks { get; set; } = new();

    public static bool operator == (Stage lhs, Stage rhs) {
        Assert.That(lhs.chunks.Count == rhs.chunks.Count);
        bool equalChunks = true;
        for (int i = 0; i < lhs.chunks.Count; i++) {
            equalChunks &= lhs.chunks[i] == rhs.chunks[i];
        }
        return lhs.start == rhs.start && lhs.end == rhs.end
            && equalChunks;
    }

    public static bool operator != (Stage lhs, Stage rhs) {
        return !(lhs == rhs);
    }
}