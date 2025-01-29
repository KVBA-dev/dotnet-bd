using Raylib_cs;
using System.Diagnostics;

namespace Game.UI;

public class LoadingElement : UIElement {
    private bool _enabled = true;
    private Stopwatch sw = new();
    public bool Enabled {
        get => _enabled;
        set
        {
            _enabled = value;
            if (_enabled) {
                sw.Start();
                return;
            }
            sw.Stop();
        }
    }
    public LoadingElement(IUIHandler parent, Rectangle rect) : base(parent, rect) {
    }

    private float EaseOut(float t) {
        return 1 - (float)Math.Pow(1 - t, 3);
    }

    private float Frac(float t) => t - (int)t;

    public override void Render() {
        if (!_enabled) {
            return;
        }
        float radius = (Rect.Width > Rect.Height ? Rect.Height : Rect.Width) * .7f;
        rl.DrawPoly(Rect.Center(), 4, radius, EaseOut(Frac((float)sw.Elapsed.TotalSeconds)) * 90, Color.White);
    }

    public override bool Update() {
        return true;
    }
}
