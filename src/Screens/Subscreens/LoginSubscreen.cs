using Game.UI;
using System.Security.Cryptography;
using System.Net.Http.Json;
using System.Text.Json;

namespace Game.Screens;
public sealed class LoginSubscreen : ISubScreen, IUIHandler {
    public UIElement FocusedElement => elements.Single(e => e.Focused);
    public IScreen Parent { get; init; }
    public Action OnBack { get; set; }
    public GameState State { get; init; }
    public readonly List<UIElement> elements;

    InputField inp_username;
    InputField inp_password;

    Label lbl_username;
    Label lbl_password;
    Label lbl_status;

    Button btn_back;
    Button btn_login;
    Button btn_register;

    LoadingElement loading;

    Task<bool>? currentTask;

    public LoginSubscreen(IScreen parent, GameState state) {
        State = state;
        Parent = parent;

        currentTask = null;

        lbl_username = new(this, new(), "Username");
        lbl_password = new(this, new(), "Password");
        lbl_status = new(this, new(), string.Empty);

        lbl_username.Alignment = Alignment.CenterRight;
        lbl_password.Alignment = Alignment.CenterRight;

        inp_username = new(this, new());
        inp_password = new(this, new());
        inp_password.HideContent = true;

        loading = new(this, new());

        btn_back = new Button(this, new(), "Back").OnClicked(() => { OnBack?.Invoke(); });
        btn_back.Focused = true;

        btn_login = new Button(this, new(), "Log in").OnClicked(() => {
            if (currentTask is not null) {
                return;
            }
            lbl_status.Caption = "";
            currentTask = Task.Run(LogIn);
        });

        btn_register = new Button(this, new(), "Register").OnClicked(() => {
            if (currentTask is not null) {
                return;
            }
            lbl_status.Caption = "";
            currentTask = Task.Run(Register);
        });

        elements = [
            lbl_username,
            lbl_password,
            lbl_status,
            inp_username,
            inp_password,
            btn_back,
            btn_login,
            btn_register,
            loading,
        ];

    }

    public void SetFocused(UIElement element) {
        if (elements.Any(e => e.Focused)) {
            FocusedElement.Focused = false;
        }
        element.Focused = true;
    }
    public void Update() {
        lbl_username.Rect = UISpecs.ScreenRect.RelativeRect(.25f, .4f, .2f, .05f);
        lbl_password.Rect = UISpecs.ScreenRect.RelativeRect(.25f, .5f, .2f, .05f);
        lbl_status.Rect = UISpecs.ScreenRect.RelativeRect(0, .75f, 1, .05f);

        inp_username.Rect = UISpecs.ScreenRect.RelativeRect(.5f, .4f, .2f, .05f);
        inp_password.Rect = UISpecs.ScreenRect.RelativeRect(.5f, .5f, .2f, .05f);

        btn_back.Rect = UISpecs.ScreenRect.RelativeRect(.45f, .9f, .1f, .05f);
        btn_login.Rect = UISpecs.ScreenRect.RelativeRect(.28f, .6f, .2f, .1f);
        btn_register.Rect = UISpecs.ScreenRect.RelativeRect(.52f, .6f, .2f, .1f);

        loading.Rect = lbl_status.Rect;
        loading.Enabled = currentTask is not null;

        if (currentTask is not null && currentTask.IsCompleted) {
            if (!currentTask.Result) {
                lbl_status.Caption = "something went wrong";
            }
            else {
                Parent.screens.Pop();
                LevelBrowserSubscreen browser = new(Parent, State);
                browser.OnBack = OnBack;
                Parent.screens.Push(browser);
                return;
            }
            currentTask = null;
        }

        foreach (UIElement e in elements) {
            e.Update();
        }
    }

    private Stream StreamFromString(string s) {
        MemoryStream stream = new();
        StreamWriter writer = new(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    async Task<bool> Register() {
        using SHA3_256 sha = SHA3_256.Create();
        using Stream passwordStream = StreamFromString(inp_password.Text);
        using HttpClient client = new();
        HttpRequestMessage req = new(HttpMethod.Post, OnlineScreen.GetRoute("users"));

        byte[] passhash = await sha.ComputeHashAsync(passwordStream);

        req.Content = JsonContent.Create(new {
            username = inp_username.Text,
            passhash = Convert.ToBase64String(passhash),
        });

        HttpResponseMessage res = await client.SendAsync(req);
        int id = JsonSerializer.Deserialize<LoginResponse>(await res.Content.ReadAsStringAsync()).id;

        State.loggedUser = new User.LoggedUser(inp_username.Text, id);
        return res.IsSuccessStatusCode;
    }

    class LoginResponse {
        public int id { get; set; }
    }

    async Task<bool> LogIn() {
        using SHA3_256 sha = SHA3_256.Create();
        using Stream passwordStream = StreamFromString(inp_password.Text);
        using HttpClient client = new();
        HttpRequestMessage req = new(HttpMethod.Post, OnlineScreen.GetRoute("login"));

        byte[] passhash = await sha.ComputeHashAsync(passwordStream);

        req.Content = JsonContent.Create(new {
            username = inp_username.Text,
            passhash = Convert.ToBase64String(passhash),
        });

        HttpResponseMessage res = await client.SendAsync(req);
        if (!res.IsSuccessStatusCode) {
            return false;
        }
        int id = JsonSerializer.Deserialize<LoginResponse>(await res.Content.ReadAsStringAsync()).id;

        State.loggedUser = new User.LoggedUser(inp_username.Text, id);

        return true;
    }

    public void Render() {
        rl.BeginDrawing();
        rl.ClearBackground(ColorPalette.Default.background);

        foreach (UIElement e in elements) {
            e.Render();
        }

        rl.EndDrawing();
    }
}
