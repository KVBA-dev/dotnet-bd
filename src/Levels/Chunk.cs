namespace Game.Levels;

public class Chunk {
    public const int CHUNK_SIZE = 16;
    public int chunkX { get; set; }
    public int chunkY { get; set; }
    public TileType[,] tiles = new TileType[CHUNK_SIZE, CHUNK_SIZE];
    public string[] Tiles
    {
        get
        {
            string[] strings = new string[CHUNK_SIZE];
            int x, y = 0;
            ulong temp;
            while (y < CHUNK_SIZE)
            {
                x = 0;
                temp = 0;
                while (x < CHUNK_SIZE)
                {
                    temp = (temp << 4) | (byte)tiles[x, y];
                    x++;
                }
                strings[y] = $"0x{temp:X16}";
                y++;
            }
            return strings;
        }
        set
        {
            Assert.That(value.Length == CHUNK_SIZE);
            int y = 0;
            int x;
            ulong temp;
            while (y < CHUNK_SIZE)
            {
                temp = Convert.ToUInt64(value[y], 16);
                x = CHUNK_SIZE - 1;
                while (x >= 0)
                {
                    tiles[x, y] = (TileType)(temp & 0xF);
                    temp >>= 4;
                    x--;
                }
                y++;
            }
        }
    }
    public static bool operator == (Chunk lhs, Chunk rhs) {
        return lhs.chunkX == rhs.chunkX
            && lhs.chunkY == rhs.chunkY
            && lhs.tiles.Rank == rhs.tiles.Rank
            && Enumerable.Range(0, lhs.tiles.Rank).All(dim => lhs.tiles.GetLength(dim) == rhs.tiles.GetLength(dim))
            && lhs.tiles.Cast<TileType>().SequenceEqual(rhs.tiles.Cast<TileType>());
    }

    public static bool operator != (Chunk lhs, Chunk rhs) {
        return !(lhs == rhs);
    }

    public Chunk Clone() {
        return new() {
            chunkX = chunkX,
            chunkY = chunkY,
            tiles = (TileType[,])tiles.Clone(),
        };

    }
}
