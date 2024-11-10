using Raylib_cs;

namespace Game.Resources;

public class IconRegistry : IRegistry {
    public static readonly IconRegistry Reg = new();

    public Texture2D File { get; private set; }
    public Texture2D Directory { get; private set; }
    private IconRegistry() {}
    public static void Load() {
        Reg.File = rl.LoadTexture("./res/ui/file.png");
        Reg.Directory = rl.LoadTexture("./res/ui/dir.png");
    }

    public static void Unload() {
        rl.UnloadTexture(Reg.File);
        rl.UnloadTexture(Reg.Directory);
    }
}