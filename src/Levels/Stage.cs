using System.Numerics;
namespace Game.Levels;

public class Stage {
    public Vector2 start { get; set; }
    public Vector2 end { get; set; }
    public string name { get; set; } = string.Empty;
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

    public Stage Clone() {
        return new() {
            start = start,
            end = end,
            name = name,
            chunks = chunks.Select(c => c.Clone()).ToList(),
        };
    }

    public void CopyFrom(Stage other) {
        start = other.start;
        end = other.end;
        name = other.name;
        chunks = other.chunks.Select(c => c.Clone()).ToList();
    }
}