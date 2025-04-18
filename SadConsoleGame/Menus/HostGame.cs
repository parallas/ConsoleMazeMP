using SadConsole.Input;
using SadConsole.UI;

namespace SadConsoleGame.Menus;

public class HostGame : BaseMenu
{
    private ScreenSurface _surface;

    private CustomTextBox _portTextBox;

    public HostGame(ScreenSurface surface) : base(surface, 32, 16)
    {
        _surface = new ScreenSurface(Width, Height);
        _surface.Fill(Color.White, Color.Transparent, 0);
        Children.Add(_surface);

        _surface.Print(0, 0, "Host Game");

        ControlHost controls = new() {ClearOnAdded = false};
        _surface.SadComponents.Add(controls);

        _portTextBox = new CustomTextBox(5) { Position = (0, 4) };
        controls.Add(_portTextBox);
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Center();
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        return _portTextBox.ProcessKeyboard(keyboard);
    }
}
