namespace Game.User;

public sealed class LoggedUser {
    public readonly string userName;
    public readonly int userId;

    public LoggedUser(string userName, int id) {
        this.userName = userName;
        userId = id;
    }
}
