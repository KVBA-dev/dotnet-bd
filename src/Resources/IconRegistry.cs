using Raylib_cs;

namespace Game.Resources;

public class IconRegistry : IRegistry {
    public static readonly IconRegistry Reg = new();

    public Texture2D File { get; private set; }
    public Texture2D Directory { get; private set; }
    public Texture2D EditorExit { get; private set; }
    public Texture2D EditorSave { get; private set; }
    public Texture2D EditorPlay { get; private set; }
    public Texture2D EditorStop { get; private set; }
    private IconRegistry() {}
    public static void Load() {
        Reg.File = rl.LoadTexture("./res/ui/file.png");
        Reg.Directory = rl.LoadTexture("./res/ui/dir.png");
        Reg.EditorExit = rl.LoadTexture("./res/ui/editor_exit.png");
        Reg.EditorSave = rl.LoadTexture("./res/ui/editor_save.png");
        Reg.EditorPlay = rl.LoadTexture("./res/ui/editor_play.png");
        Reg.EditorStop = rl.LoadTexture("./res/ui/editor_stop.png");
    }

    public static void Unload() {
        rl.UnloadTexture(Reg.File);
        rl.UnloadTexture(Reg.Directory);
        rl.UnloadTexture(Reg.EditorExit);
        rl.UnloadTexture(Reg.EditorSave);
        rl.UnloadTexture(Reg.EditorPlay);
        rl.UnloadTexture(Reg.EditorStop);
    }
}