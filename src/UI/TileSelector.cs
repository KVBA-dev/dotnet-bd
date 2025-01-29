using Raylib_cs;
using Game.Levels;
using Game.Screens;
using Game.Resources;

namespace Game.UI;

public sealed class TileSelector : UIElement {
    private List<ImageButton> buttons = new();
    public Action<TileType, ImageButton> OnTileSelected = (_, _) => { };
    private int selected = 0;
    public TileSelector(IUIHandler parent, Rectangle rect) : base(parent, rect) {
        ImageButton button;

        for (int y = 0; y < 4; y++) {
            for (int x = 0; x < 4; x++) {
                int idx = y * 4 + x;
                Texture2D tex = new();
                try {
                    tex = TextureRegistry.Tiles[(TileType)idx];
                }
                catch {}
                button = new(parent, rect.RelativeRect(x * .25f, y * .25f, .25f, .25f), tex);
                button.OnClick += () => { 
                    OnTileSelected?.Invoke((TileType)idx, button); 
                };
                button.palette = ColorPalette.Editor;
                buttons.Add(button);
            }
        }

        (parent as StageEditorSubscreen)?.AddElements(buttons);
    }

    public Rectangle GetSelectedRect(int selected) {
        return buttons[selected].Rect;
    }

    public override void Render() {
        foreach (ImageButton button in buttons) {
            button.Render();
        }
    }

    public override bool Update() {
        bool output = false;
        for (int y = 0; y < 4; y++) {
            for (int x = 0; x < 4; x++) {
                int idx = y * 4 + x;
                buttons[idx].Rect = Rect.RelativeRect(x * .25f, y * .25f, .25f, .25f);
                buttons[idx].ImageRect = buttons[idx].Rect.RelativeRect(.15f, .15f, .7f, .7f);
            }
        }
        foreach (ImageButton button in buttons) {
            output |= button.Update();
        }
        return output;
    }
}