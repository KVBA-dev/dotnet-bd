using Raylib_cs;

namespace Game;

public struct InputMap {
    public KeyboardKey keyLeft { get; set; }
    public KeyboardKey keyRight { get; set; }
    public KeyboardKey keyJump { get; set; }

    public static void Remap(ref KeyboardKey key) {
        do {
            key = (KeyboardKey)rl.GetKeyPressed();
        } while (key != KeyboardKey.Null);
    }
}