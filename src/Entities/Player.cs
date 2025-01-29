using Game.Levels;
using Game.Resources;

namespace Game.Entities;

public sealed class Player : Entity {
    float gravity;
    float initJumpVel;
    bool canJump = false;
    public Action<Player> OnWin;
    public Player(GameState state, Stage stage) : base(state, stage) {
        controller = new PlayerController();
        controller.Enabled = true;
        (gravity, initJumpVel) = GetJumpParams(3, .6f);
        initJumpVel *= 4;
        renderer.texture = TextureRegistry.Reg.Player;
    }

    public override void Update() {
        base.Update();
        VelX += controller.Horizontal;
        VelX *= .9f;
        float newX = X + VelX * Time.DeltaTime;
        // check collision
        if (!CheckCollisionHorizontal(new(newX, Y))) {
            X = newX;
        }
        else {
            VelX *= -.2f;
        }

        if (canJump && controller.Vertical > 0) {
            VelY = initJumpVel;
            canJump = false;
        }
        VelY += gravity;
        float newY = Y + VelY * Time.DeltaTime;
        // check collision
        if (!CheckCollisionVertical(new(X, newY))) {
            Y = newY;
        }
        else {
            if (VelY > 0) {
                canJump = true;
            }
            VelY = 0;
        }

        if ((Pos - currentStage.end).LengthSquared() <= 1) {
            OnWin?.Invoke(this);
        }
    }

    private static (float gravity, float initVel) GetJumpParams(float maxHeight, float time) {
        time *= 4;
        maxHeight *= 4;
        float gravity = maxHeight / (2 * time * time);
        float initVel = (float)-Math.Sqrt(2 * maxHeight * gravity);
        return (gravity, initVel);
    }
}
