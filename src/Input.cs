using Raylib_cs;
using System.Numerics;

namespace Game;

public static class Input {
    public static bool Left { get; private set; }
    public static bool Right { get; private set; }
    public static bool Jump { get; private set; }
    public static Vector2 MousePosition => rl.GetMousePosition();
    public static float MoseScroll => rl.GetMouseWheelMove();


    public static bool UILeft { get; private set; }
    public static bool UIRight { get; private set; }
    public static bool UIUp { get; private set; }
    public static bool UIDown { get; private set; }
    public static bool UIConfirm { get; private set; }
    public static bool UICancel { get; private set; }

    public static void UpdateAxes(InputMap inputMap) {
        Left = rl.IsKeyDown(inputMap.keyLeft);
        Right = rl.IsKeyDown(inputMap.keyRight);
        Jump = rl.IsKeyPressed(inputMap.keyJump);

        UILeft = rl.IsKeyPressed(KeyboardKey.A) || rl.IsKeyPressedRepeat(KeyboardKey.A) || rl.IsKeyPressed(KeyboardKey.Left) || rl.IsKeyPressedRepeat(KeyboardKey.Left);
        UIRight = rl.IsKeyPressed(KeyboardKey.D) || rl.IsKeyPressedRepeat(KeyboardKey.D) || rl.IsKeyPressed(KeyboardKey.Right) || rl.IsKeyPressedRepeat(KeyboardKey.Right);
        UIUp = rl.IsKeyPressed(KeyboardKey.W) || rl.IsKeyPressedRepeat(KeyboardKey.W) || rl.IsKeyPressed(KeyboardKey.Up) || rl.IsKeyPressedRepeat(KeyboardKey.Up);
        UIDown = rl.IsKeyPressed(KeyboardKey.S) || rl.IsKeyPressedRepeat(KeyboardKey.S) || rl.IsKeyPressed(KeyboardKey.Down) || rl.IsKeyPressedRepeat(KeyboardKey.Down);

        UIConfirm = rl.IsKeyPressed(KeyboardKey.Enter);
        UICancel = rl.IsKeyPressed(KeyboardKey.Escape);
    }

    public static bool MouseInRect(Rectangle rect) {
        return rl.CheckCollisionPointRec(MousePosition, rect);
    }

    public static Vector2 MouseToWorldSpace(Camera2D camera) {
        return rl.GetScreenToWorld2D(MousePosition, camera);
    }
}