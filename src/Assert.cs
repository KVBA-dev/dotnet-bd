namespace Game;

public static class Assert {
    public static void That(bool condition) {
        if (!condition) {
            throw new AssertionException("Assertion failed");
        }
    }
}

public sealed class AssertionException : Exception {
    public AssertionException(string msg) : base(msg) {}
}