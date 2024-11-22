using System.Numerics;
using Raylib_cs;
namespace Game;

public static class Constants {
    public const int REFERENCE_WIDTH = 800;
    public const string LEVEL_FILE_EXTENSION = "tmv";
    public static readonly Color SelectedColour = rl.ColorAlpha(Color.SkyBlue, .5f);
    public const int TILE_SIZE = 40;
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => a + t * (b - a);

    public const byte COL_SOLID = 0x1;
    public const byte COL_KILLING = 0x2;
    public const byte COL_NO_BOTTOM = 0x4;
    public const byte COL_LOW_FRICTION = 0x8;
    public const byte COL_BOUNCY = 0x10;
    public const byte COL_BREAKABLE = 0x20;
}