using System.Numerics;
using Game.Resources;
using Raylib_cs;

namespace Game.UI;

public class FileExplorerEntry {
    public string pwd;
    public string path;
    public bool isDirectory;
    public Rectangle rect;
    public bool hovered;
    public bool selected;

    public FileExplorerEntry(string pwd, string path, bool isDirectory) {
        this.path = path;
        this.pwd = pwd;
        this.isDirectory = isDirectory;
    }
}

public class FileExplorer : UIElement {
    private RenderTexture2D canvas;
    public bool Initialised { get; private set; }
    private string rootPath;
    private string pwd;
    private static readonly string[] first = { ".." };
    private int topIdx = 0;
    public event Action OnLevelOpen = () => { };

    private readonly List<FileExplorerEntry> explorerEntries = new();
    public string SelectedLevel { get; private set; } = string.Empty;

    private string DisplayPath {
        get {
            return $"{pwd.Substring(rootPath.Length)}\\";
        }
    }
    public FileExplorerEntry? SelectedEntry {
        get {
            var entry = explorerEntries.Where(e => e.selected);
            if (entry.Count() == 0) {
                return null;
            }
            return entry.Single();
        }
    }

    public FileExplorer(IUIHandler parent, Rectangle rect, string rootPath) : base(parent, rect) {
        Initialised = false;
        this.rootPath = Environment.CurrentDirectory + rootPath;
        Console.WriteLine(this.rootPath);
        pwd = this.rootPath;

        if (!Directory.Exists(pwd)) {
            Directory.CreateDirectory(pwd);
        }

        OpenPWD();
    }

    public void InitCanvas() {
        canvas = rl.LoadRenderTexture((int)Rect.Width, (int)Rect.Height);
        Initialised = true;
    }

    public static Rectangle GetTextureRext(Texture2D texture) {
        return new(0, 0, texture.Width, texture.Height);
    }

    public void OpenPWD() {
        explorerEntries.Clear();

        IEnumerable<string> files = Directory.GetFiles(pwd)
                                  .Select(p => p.Split('\\')[^1]);
        IEnumerable<string> dirs = Directory.GetDirectories(pwd)
                                 .Select(p => p.Split('\\')[^1]);
        if (pwd != rootPath) {
            dirs = first.Concat(dirs);
        }

        foreach(string p in dirs) {
            explorerEntries.Add(new(pwd, p, true));
        }
        foreach(string p in files) {
            explorerEntries.Add(new(pwd, p, false));
        }
    }

    public override bool Update() {
        if (!Focused) {
            return false;
        }

        int size = (int)(15 * UISpecs.Scale);
        int i = -topIdx * size;
        Rectangle hitbox;
        foreach (FileExplorerEntry e in explorerEntries) {
            e.rect = new(Rect.X, Rect.Y + i, Rect.Width, size);
            hitbox = e.rect;
            // hitbox = RectIntersect(Rect, e.rect);
            e.hovered = false;
            if (!rl.CheckCollisionPointRec(rl.GetMousePosition(), hitbox)) {
                i += size;
                continue;
            }
            e.hovered = true;
            if (rl.IsMouseButtonPressed(MouseButton.Left)) {
                if (e.selected) {
                    EntryClick(e);
                    return true;
                }
                if (SelectedEntry is not null) {
                    SelectedEntry.selected = false;
                }
                e.selected = true;
            }
            i += size;
        }
        return false;
    }

    public void EntryClick(FileExplorerEntry entry) {
        if (entry.isDirectory) {
            if (entry.path.Equals("..")) {
                pwd = Path.Combine(pwd, "..");
                pwd = Path.GetFullPath(pwd);
                OpenPWD();
                return;
            }
            pwd = entry.pwd + '\\' + entry.path;
            OpenPWD();
            return;
        }
        if (entry.path.Split('.')[^1].Equals(Constants.LEVEL_FILE_EXTENSION)) {
            SelectedLevel = Path.Combine(pwd, entry.path);
            OnLevelOpen?.Invoke();
        }
    }

    public override void Render() {

        int size = (int)(15 * UISpecs.Scale);

        rl.BeginTextureMode(canvas); {
            rl.ClearBackground(rl.ColorAlpha(Color.Blue, .5f));

            int i = -topIdx * size;
            Color tint;
            Texture2D icon;
            foreach (FileExplorerEntry e in explorerEntries) {
                tint = Color.White;
                if (e.isDirectory) {
                    icon = IconRegistry.Reg.Directory;
                }
                else {
                    icon = IconRegistry.Reg.File;
                    if (e.path.Split('.')[^1].Equals(Constants.LEVEL_FILE_EXTENSION)) {
                        tint = Color.Green;
                    }
                }
                Rectangle rect = e.rect;
                rect.Position -= Rect.Position;
                if (e.hovered) {
                    rl.DrawRectangleRec(rect, rl.ColorAlpha(Color.White, .2f));
                }
                if (e.selected) {
                    rl.DrawRectangleRec(rect, rl.ColorAlpha(Color.White, .4f));
                }


                rl.DrawTexturePro(icon, GetTextureRext(icon), new(0, i, size, size), new(0, 0), 0f, tint);
                rl.DrawText(e.path, (int)(size * 1.3f), i, size, Color.White);
                i += size;
            }
        }
        rl.EndTextureMode();

        Rectangle canvasRect = GetTextureRext(canvas.Texture);
        canvasRect.Height *= -1;
        rl.DrawTexturePro(canvas.Texture, canvasRect, Rect, new(0, 0), 0f, Color.White);

        rl.DrawText(DisplayPath, 0, UISpecs.Height - size, size, Color.White);
    }

    

    public static Rectangle RectIntersect(Rectangle a, Rectangle b) {
        Vector2 topLeft = new();
        Vector2 bottomRight = new();

        topLeft.X = a.X > b.X ? a.X : b.X;
        topLeft.Y = a.Y > b.Y ? a.Y : b.Y;
        
        bottomRight.X = a.X + a.Width > b.X + b.Width ? a.X + a.Width : b.X + b.Width;
        bottomRight.Y = a.Y + a.Height > b.Y + b.Height ? a.Y + a.Height : b.Y + b.Height;

        float w = bottomRight.X - topLeft.X;
        float h = bottomRight.Y - topLeft.Y;

        if (w < 0) {
            w = 0;
        }
        if (h < 0) {
            h = 0;
        }
        return new(topLeft, w, h);
    }
}