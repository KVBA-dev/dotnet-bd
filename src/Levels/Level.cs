using System.Text.Json;

namespace Game.Levels;

public class Level {
    public string name { get; set; } = string.Empty;
    public string author { get; set; } = string.Empty;
    public List<Stage> stages { get; set; } = new();

    public static bool operator == (Level lhs, Level rhs) {
        Assert.That(lhs.stages.Count == rhs.stages.Count);
        bool equalStages = true;
        for (int i = 0; i < lhs.stages.Count; i++) {
            equalStages &= lhs.stages[i] == rhs.stages[i];
        }

        return lhs.name == rhs.name
            && lhs.author == rhs.author
            && equalStages;
    }

    public static bool operator != (Level lhs, Level rhs) {
        return !(lhs == rhs);
    }

}