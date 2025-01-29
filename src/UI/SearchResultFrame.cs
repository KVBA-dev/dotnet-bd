using GameBE.Data;
using Raylib_cs;
namespace Game.UI;

public sealed class SearchResultFrame : UIElement {

    public readonly SearchResultField[] results = new SearchResultField[10];

    public SearchResultFrame(IUIHandler parent, Rectangle rect) : base(parent, rect) {
        for (int i = 0; i < results.Length; i++) {
            results[i] = new(parent, new());
        }
    }

    public override void Render() {
        foreach (SearchResultField r in results) {
            r.Render();
        }
    }

    public override bool Update() {
        int i = 0;
        bool result = false;
        foreach (SearchResultField r in results) {
            float x = (float)Math.Round((float)i / 10f, MidpointRounding.AwayFromZero) * .5f;
            float y = (float)i / 5f - i / 5;
            r.Rect = Rect.RelativeRect(x, y, .5f, .2f);

            result |= r.Update();
            i += 1;
        }
        return result;
    }

    public void SetResults(SearchResult results) {
        int i = 0;
        Console.WriteLine("settings results...");
        foreach (LevelInfo info in results.levels) {
            this.results[i].Info = info;
            i += 1;
            if (i == this.results.Length) {
                break;
            }
        }
    }

    public void ClearResults() {
        foreach (SearchResultField r in results) {
            r.Info = null;
        }
    }
}
