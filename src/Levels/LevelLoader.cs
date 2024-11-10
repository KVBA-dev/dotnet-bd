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
        Level? level;
        try {
            level = JsonSerializer.Deserialize<Level>(File.ReadAllText(path), options);
        }
        catch (JsonException e) {
            Console.WriteLine($"ERROR: Cannot read the file: {e.Message}");
            level = null;
        }
        return level;
    }

    public static bool SaveLevel(string path, Level level) {
        Task<bool> task = Task.Run(() => SaveAsync(path, level));
        while (!task.IsCompleted) {};
        return task.Result;
    }


    private static async Task<bool> SaveAsync(string path, Level level) {
        try {
            MemoryStream mem = new();
            await JsonSerializer.SerializeAsync(mem, level, options);
            File.WriteAllBytes(path, mem.GetBuffer()[..(int)mem.Length]);
        }
        catch {
            return false;
        }
        return true;
    } 
}