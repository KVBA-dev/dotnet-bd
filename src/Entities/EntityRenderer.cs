using System.Numerics;
using Game.UI;
using Raylib_cs;

namespace Game.Entities;

public sealed class EntityRenderer {
    public Texture2D texture;
    public bool flipHorizontally = false;
    public void Render(Entity entity) {
        Vector2 entityPos = entity.Pos;
        Rectangle target = new();
        target.Position = entityPos * Constants.TILE_SIZE;
        target.Size = new Vector2(Constants.TILE_SIZE, Constants.TILE_SIZE) * entity.size;

        Rectangle source = texture.Rectangle();
        if (flipHorizontally) {
            source.Width *= -1;
        }

        rl.DrawTexturePro(texture, source, target, Vector2.Zero, 0, Color.White);
    }
}