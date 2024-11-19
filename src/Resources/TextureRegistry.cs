using Game.Levels;
using Raylib_cs;

namespace Game.Resources;

public class TextureRegistry : IRegistry
{
    public static readonly TextureRegistry Reg = new();
    private TextureRegistry() { }

    public Texture2D EmptyTile { get; private set; }
    public Texture2D FloorTile { get; private set; }
    public Texture2D KillingTile { get; private set; }
    public Texture2D BreakableTile { get; private set; }
    public static Dictionary<TileType, Texture2D> Tiles => new(){
        {TileType.Empty, Reg.EmptyTile},
        {TileType.Floor, Reg.FloorTile},
        {TileType.Killing, Reg.KillingTile},
        {TileType.Breakable, Reg.BreakableTile},
    };



    public static void Load() {
        Reg.EmptyTile = rl.LoadTexture("./res/level/empty.png");
        Reg.FloorTile = rl.LoadTexture("./res/level/floor.png");
        Reg.KillingTile = rl.LoadTexture("./res/level/killing.png");
        Reg.BreakableTile = rl.LoadTexture("./res/level/breakable.png");
    }

    public static void Unload() {
        rl.UnloadTexture(Reg.EmptyTile);
        rl.UnloadTexture(Reg.FloorTile);
        rl.UnloadTexture(Reg.KillingTile);
        rl.UnloadTexture(Reg.BreakableTile);
    }
}