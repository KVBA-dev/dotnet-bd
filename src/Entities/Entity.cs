using System.Numerics;
using Game.Levels;

namespace Game.Entities;

public abstract class Entity {
    public readonly float size = .9f;
    protected readonly GameState state;
    public IEntityController controller;
    protected EntityRenderer renderer;
    protected Stage currentStage;
    public float X { get; set; }
    public float Y { get; set; }
    public float VelX { get; set; }
    public float VelY { get; set; }

    public Vector2 Pos {
        get => new(X, Y);
        set {
            X = value.X;
            Y = value.Y;
        }
    }
    public Action<Entity> OnDeath;
    public virtual void Update() {
        controller.GetAxes();
    }
    public virtual void Render() {
        if (controller.Horizontal > 0) {
            renderer.flipHorizontally = false;
        }
        if (controller.Horizontal < 0) {
            renderer.flipHorizontally = true;
        }
        renderer.Render(this);
    }
    public void GoTo(float x, float y) => (X, Y) = (x, y);
    public void GoTo(Vector2 pos) => (X, Y) = (pos.X, pos.Y);
    public Entity(GameState state, Stage stage) {
        this.state = state;
        currentStage = stage;
        renderer = new();
    }

    public byte GetCollision(Vector2 testedPos) {
        int minX = (int)Math.Floor(testedPos.X);
        int maxX = (int)Math.Floor(testedPos.X + size);
        int minY = (int)Math.Floor(testedPos.Y);
        int maxY = (int)Math.Floor(testedPos.Y + size);
        return (byte)(currentStage.GetCollisionTileAt(minX, minY)
                    | currentStage.GetCollisionTileAt(minX, maxY)
                    | currentStage.GetCollisionTileAt(maxX, minY)
                    | currentStage.GetCollisionTileAt(maxX, maxY));
    }

    public bool CheckCollisionVertical(Vector2 testedPos) {
        byte collision = GetCollision(testedPos);
        if (collision == 0) {
            return false;
        }

        if ((collision & Constants.COL_KILLING) != 0) {
            currentStage.deathActions.Enqueue((this, OnDeath));
            return true;
        }

        if ((collision & Constants.COL_NO_BOTTOM) != 0 && VelY < 0) {
            return false;
        }

        if ((collision & Constants.COL_SOLID) != 0) {
            return true;
        }

        if ((collision & Constants.COL_BREAKABLE) != 0) {
            return true;
        }
        return false;
    }

    public bool CheckCollisionHorizontal(Vector2 testedPos) {
        byte collision = GetCollision(testedPos);
        if (collision == 0) {
            return false;
        }

        if ((collision & Constants.COL_KILLING) != 0) {
            currentStage.deathActions.Enqueue((this, OnDeath));
            return true;
        }


        if ((collision & Constants.COL_BREAKABLE) != 0) {
            currentStage.SetCollisionTileAt((int)testedPos.X, (int)testedPos.Y, 0);
            return false;
        }
        if ((collision & Constants.COL_SOLID) != 0) {
            return (collision & Constants.COL_NO_BOTTOM) == 0;
        }
        return false;
    }
}
