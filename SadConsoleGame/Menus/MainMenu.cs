using SadConsole.Input;
using SadConsole.UI;
using SadConsoleGame.Menus;

namespace SadConsoleGame;

public class MainMenu : BaseMenu
{
    private ScreenSurface _surface;
    private ControlHost _host;

    private List<KeyInputButton> _keyInputButtons = new List<KeyInputButton>();

    public MainMenu(ScreenSurface screenSurface) : base(screenSurface, 32, 16)
    {
        _surface = new ScreenSurface(Width, Height);
        _surface.Fill(Color.White, Color.Transparent, 0);
        Children.Add(_surface);

        _host = new ControlHost
        {
            ClearOnAdded = false
        };
        _surface.SadComponents.Add(_host);

        _keyInputButtons.Add(new KeyInputButton(Width, Keys.H, "Host", MainMenuManager.GoToHostGame));
        _keyInputButtons.Add(new KeyInputButton(Width, Keys.J, "Join", () => {}));
        _keyInputButtons.Add(new KeyInputButton(Width, Keys.A, "About", () => {}));
        _keyInputButtons.Add(new KeyInputButton(Width, Keys.O, "Options", () => {}));
        _keyInputButtons.Add(new KeyInputButton(Width, Keys.Q, "Quit", () => { Environment.Exit(0); }));
        for (var index = 0; index < _keyInputButtons.Count; index++)
        {
            var keyInputButton = _keyInputButtons[index];
            keyInputButton.Position = (0, Height - 1 - (_keyInputButtons.Count - index) * 2);
            _surface.Children.Add(keyInputButton);
        }

        _surface.Print(0, 0, "Mainframe / Grid Computing");

        IsFocused = true;
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Center();
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
