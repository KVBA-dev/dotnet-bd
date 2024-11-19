using Raylib_cs;
namespace Game;

public static class Constants {
    public const int REFERENCE_WIDTH = 800;
    public const string LEVEL_FILE_EXTENSION = "tmv";
    public static readonly Color SelectedColour = rl.ColorAlpha(Color.SkyBlue, .5f);
    public const int TILE_SIZE = 40;
}