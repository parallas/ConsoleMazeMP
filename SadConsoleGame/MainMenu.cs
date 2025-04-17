using SadConsole.Input;
using SadConsole.UI;

namespace SadConsoleGame;

public class MainMenu : ScreenObject
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private ScreenSurface _surface;
    private ControlHost _host;

    private List<KeyInputButton> _keyInputButtons = new List<KeyInputButton>();
    public MainMenu(int width, int height)
    {
        Width = width;
        Height = height;

        _surface = new ScreenSurface(width, height);
        _surface.Fill(Color.White, Color.Transparent, 0);
        Children.Add(_surface);

        _host = new ControlHost();
        _host.ClearOnAdded = false;
        _surface.SadComponents.Add(_host);

        _keyInputButtons.Add(new KeyInputButton(width, Keys.H, "Host", () => {}));
        _keyInputButtons.Add(new KeyInputButton(width, Keys.J, "Join", () => {}));
        _keyInputButtons.Add(new KeyInputButton(width, Keys.A, "About", () => {}));
        _keyInputButtons.Add(new KeyInputButton(width, Keys.O, "Options", () => {}));
        _keyInputButtons.Add(new KeyInputButton(width, Keys.Q, "Quit", () => { Environment.Exit(0); }));
        for (var index = 0; index < _keyInputButtons.Count; index++)
        {
            var keyInputButton = _keyInputButtons[index];
            keyInputButton.Position = (0, height - 1 - (_keyInputButtons.Count - index) * 2);
            _host.Add(keyInputButton);
        }

        _surface.Print(0, 0, "Mainframe / Grid Computing");

        IsFocused = true;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        foreach (var keyInputButton in _keyInputButtons)
        {
            keyInputButton.ProcessKeyboard(keyboard);
        }
        return base.ProcessKeyboard(keyboard);
    }
}
