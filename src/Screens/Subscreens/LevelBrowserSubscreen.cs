using Game.UI;
using Raylib_cs;
using GameBE.Data;
using System.Net.Http.Json;
using System.Text.Json;

namespace Game.Screens;

public sealed class LevelBrowserSubscreen : ISubScreen, IUIHandler {
    public IScreen Parent { get; init; }
    public Action OnBack { get; set; }
    public GameState State { get; init; }

    public readonly List<UIElement> elements;

    Button btn_back;
    InputField inp_search;
    SearchResultFrame searchResults;

    public UIElement FocusedElement => elements.Single(e => e.Focused);

    Task<bool>? currentTask;

    public LevelBrowserSubscreen(IScreen parent, GameState state) {
        Parent = parent;
        State = state;
        OnBack = () => { };
        btn_back = new Button(this, new(), "Back").OnClicked(() => {
            OnBack.Invoke();
        });

        searchResults = new(this, new());

        foreach (SearchResultField f in searchResults.results) {
            f.OnClick = info => {
                LevelInfoSubscreen levelInfo = new(parent, state, info);
                levelInfo.OnBack = () => {
                    parent.screens.Pop();
                };
                parent.screens.Push(levelInfo);
            };
        }

        inp_search = new(this, new());
        inp_search.OnEditingEnd += (text) => {
            if (currentTask is not null) {
                return;
            }
            searchResults.ClearResults();
            currentTask = Task.Run(Search);
        };

        inp_search.Focused = true;
        inp_search.TextSize = 15;

        currentTask = null;

        elements = [
            btn_back,
            inp_search,
            searchResults,
        ];
    }

    public void Render() {
        rl.BeginDrawing();
        rl.ClearBackground(ColorPalette.Default.background);

        foreach (UIElement e in elements) {
            e.Render();
        }
        rl.EndDrawing();
    }

    public void SetFocused(UIElement element) {
        FocusedElement.Focused = false;
        element.Focused = true;
    }

    public void Update() {
        btn_back.Rect = UISpecs.ScreenRect.RelativeRect(.45f, .9f, .1f, .05f);
        Rectangle searchRect = UISpecs.ScreenRect.RelativeRect(.1f, 0f, 0.8f, 0.05f);
        searchRect.Y = (int)(inp_search.TextSize * UISpecs.Scale);
        inp_search.Rect = searchRect;

        searchResults.Rect = UISpecs.ScreenRect.RelativeRect(.1f, .1f, .8f, .8f);

        if (currentTask is not null && currentTask.IsCompleted) {
            if (!currentTask.Result) {
                // TODO: failed to fetch levels
            }
            currentTask = null;
        }

        foreach (UIElement e in elements) {
            e.Update();
        }
    }

    private async Task<bool> Search() {
        HttpClient client = new();
        HttpRequestMessage req = new(HttpMethod.Post, OnlineScreen.GetRoute("search"));

        SearchRequest search = new() {
            query = inp_search.Text,
            type = 0, // NOTE: unused, but we have to declare it
            page = 1,
        };

        req.Content = JsonContent.Create(search);

        HttpResponseMessage res = await client.SendAsync(req);
        if (res.IsSuccessStatusCode) {
            SearchResult? results = JsonSerializer.Deserialize<SearchResult>(await res.Content.ReadAsStringAsync());
            if (results is null) {
                return false;
            }
            searchResults.SetResults(results);
        }
        return res.IsSuccessStatusCode;
    }
}
