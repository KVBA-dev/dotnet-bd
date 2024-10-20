using Raylib_cs;

namespace Game.Resources;

public class IconRegistry : IRegistry {
    public static readonly IconRegistry Reg = new();

    public Texture2D File { get; private set; }
    public Texture2D Directory { get; private set; }

    private IconRegistry() {}
    public void Load() {
        File = rl.LoadTexture("./res/ui/file.png");
        Directory = rl.LoadTexture("./res/ui/dir.png");
    }

    public void Unload() {
        rl.UnloadTexture(File);
        rl.UnloadTexture(Directory);
    }
}