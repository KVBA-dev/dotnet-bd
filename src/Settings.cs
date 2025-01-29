using Raylib_cs;

namespace Game;

public static class Settings {
    public static InputMap MainMap = new() {
        keyLeft = KeyboardKey.A,
        keyJump = KeyboardKey.Space,
        keyRight = KeyboardKey.D,
    };
}