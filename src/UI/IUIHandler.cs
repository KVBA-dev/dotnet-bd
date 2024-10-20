namespace Game.UI;

public interface IUIHandler {
    public UIElement FocusedElement { get; }
    public void SetFocused(UIElement element);
}