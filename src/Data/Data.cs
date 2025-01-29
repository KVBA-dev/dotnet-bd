namespace GameBE.Data;

public sealed class UserRequest {
    public string username { get; set; } = string.Empty;
    public string passhash { get; set; } = string.Empty;
}

public sealed class LevelCreationInfo {
    public string levelname { get; set; } = string.Empty;
    public int creatorid { get; set; }
    public string level { get; set; } = string.Empty;
}

public sealed class SearchRequest {
    public int type { get; set; }
    public string query { get; set; } = string.Empty;
    public int page { get; set; }
}

public sealed class SearchResult {
    public List<LevelInfo> levels { get; set; } = [];
}

public sealed class LevelInfo {
    public int levelid { get; set; }
    public string levelname { get; set; } = string.Empty;
    public string creatorname { get; set; } = string.Empty;
    public int likes { get; set; }
    public int plays { get; set; }
    public int comments { get; set; }
}

public sealed class Comment {
    public string username { get; set; } = string.Empty;
    public int levelid { get; set; }
    public string comment { get; set; } = string.Empty;
}

public sealed class CommentPage {
    public List<Comment> comments { get; set; } = [];
}

public sealed class LevelCompletionInfo {
    public int levelid { get; set; }
    public int playerid { get; set; }
    public int attempts { get; set; }
}

public sealed class LikeInfo {
    public int levelid { get; set; }
    public int playerid { get; set; }
    public bool like { get; set; }
}

// NOTE: totally not confusing at all...

public sealed class LevelInfoRequest {
    public int levelid { get; set; }
    public int playerid { get; set; }
}

public sealed class LevelInfoResponse {
    public int attempts { get; set; }
    public bool completed { get; set; }
    public bool liked { get; set; }
}
