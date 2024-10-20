namespace Game;

public static class Assert {
    public static void That(bool condition) {
        if (!condition) {
            throw new Exception("Assertion failed");
        }
    }
}