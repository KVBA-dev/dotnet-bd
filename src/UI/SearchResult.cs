using Raylib_cs;
using GameBE.Data;
namespace Game.UI;

public sealed class SearchResultField : UIElement {

    public Action<LevelInfo> OnClick { get; set; } = _ => { };
    private LevelInfo? _info = null;
    public LevelInfo? Info {
        get => _info;
        set {
            _info = value;
            if (_info is null) {
                lbl_title.Caption = string.Empty;
                lbl_author.Caption = string.Empty;
                lbl_plays.Caption = string.Empty;
                lbl_likes.Caption = string.Empty;
                lbl_comments.Caption = string.Empty;
                return;
            }

            lbl_title.Caption = _info.levelname;
            lbl_author.Caption = _info.creatorname;
            lbl_plays.Caption = _info.plays.ToString();
            lbl_likes.Caption = _info.likes.ToString();
            lbl_comments.Caption = _info.comments.ToString();
        }
    }

    Label lbl_title;
    Label lbl_author;
    Label lbl_plays;
    Label lbl_likes;
    Label lbl_comments;

    List<Label> labels;

    public SearchResultField(IUIHandler parent, Rectangle rect) : base(parent, rect) {
        lbl_title = new(parent, new(), "");
        lbl_title.Alignment = Alignment.CenterLeft;
        lbl_title.TextSize = 20;

        lbl_author = new(parent, new(), "");
        lbl_author.Alignment = Alignment.CenterLeft;

        lbl_plays = new(parent, new(), "");
        lbl_plays.TextSize = 10;

        lbl_likes = new(parent, new(), "");
        lbl_likes.TextSize = 10;

        lbl_comments = new(parent, new(), "");
        lbl_comments.TextSize = 10;

        labels = [
            lbl_title,
            lbl_author,
            lbl_plays,
            lbl_likes,
            lbl_comments,
        ];
    }

    public override void Render() {
        if (Info is null) {
            return;
        }
        Color col = palette.background;
        if (Input.MouseInRect(Rect)) {
            col = palette.backgroundSelected;
        }
        rl.DrawRectangleRec(Rect, col);

        rl.DrawRectangleLinesEx(Rect, 8, ColorPalette.Default.border);

        foreach (Label l in labels) {
            l.Render();
        }
    }

    public override bool Update() {
        lbl_title.Rect = Rect.RelativeRect(0.05f, 0, .5f, .5f);
        lbl_author.Rect = Rect.RelativeRect(0.05f, .5f, .5f, .3f);
        lbl_plays.Rect = Rect.RelativeRect(.5f, 0, .5f, .3f);
        lbl_likes.Rect = Rect.RelativeRect(.5f, .3f, .5f, .3f);
        lbl_comments.Rect = Rect.RelativeRect(.5f, .6f, .5f, .3f);

        if (Info is null) {
            return false;
        }

        if (!rl.CheckCollisionPointRec(Input.MousePosition, Rect)) {
            return false;
        }
        if (rl.IsMouseButtonPressed(MouseButton.Left)) {
            if (_info is not null) {
                OnClick?.Invoke(_info);
            }
        }
        return true;
    }
}
