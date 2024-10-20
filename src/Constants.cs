using Raylib_cs;

namespace Game;

public static class Constants {
    public const int REFERENCE_WIDTH = 800;
    public static float UIScale => rl.GetScreenWidth() / (float)REFERENCE_WIDTH;
    public const string LEVEL_FILE_EXTENSION = "tmv";
}