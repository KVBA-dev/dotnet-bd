using System.Text.Json;

namespace Game.Levels;

public static class LevelLoader {
    private static readonly JsonSerializerOptions options = new() {
        Converters = {
            new Vector2Converter(),
        }
    };

    public static Level? LoadLevel(string path) {
        if (!File.Exists(path)) {
            return null;
        }
        return JsonSerializer.Deserialize<Level>(path, options);
    }

    public static bool SaveLevel(string path, Level level) {
        // this couldve been done better
        try {
            StreamWriter writer = new(File.OpenWrite(path));
            string json = JsonSerializer.Serialize(level, options);
            writer.Write(json);
            writer.Close();
        }
        catch {
            return false;
        }
        return true;
    }
}