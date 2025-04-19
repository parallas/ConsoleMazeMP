using SadConsole.Input;
using SadConsole.UI.Controls;

namespace SadConsoleGame.Menus;

public abstract class SequencedMenu(ScreenSurface parentSurface, int width, int height)
    : BaseMenu(parentSurface, width, height)
{
    internal ControlBase[] Elements { get; set; } = [];

    private int _elementIndex;

    public int ElementIndex
    {
        get => _elementIndex;
        set => SetFocusedElementIndex(value);
    }

    private void SetFocusedElementIndex(int index)
    {
        _elementIndex = index;
        if (_elementIndex < 0) _elementIndex = Elements.Length - 1;
        if (_elementIndex >= Elements.Length) _elementIndex = 0;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Keys.Tab))
        {
            var dir = keyboard.IsKeyDown(Keys.LeftShift) ? -1 : 1;
            ElementIndex += dir;
            return true;
        }

        if (keyboard.IsKeyReleased(Keys.Enter))
        {
            if (ElementIndex < Elements.Length - 1)
                ElementIndex++;
            else
                SubmitFinal();

            return true;
        }

        return Elements[ElementIndex].ProcessKeyboard(keyboard);
    }

    internal abstract void SubmitFinal();
}
