using SadConsole.Input;
using SadConsole.UI;

namespace SadConsoleGame.Menus;

public class JoinGame : SequencedMenu
{
    private ScreenSurface _surface;

    private CustomTextBox _addressTextBox;
    private CustomTextBox _nameTextBox;

    private ScreenSurface _placeholderSurface;

    public JoinGame(ScreenSurface surface) : base(surface, 32, 16)
    {
        _surface = new ScreenSurface(Width, Height);
        _surface.Fill(Color.White, Color.Transparent, 0);
        Children.Add(_surface);

        _surface.Print(0, 0, "Join Game");

        ControlHost controls = new() {ClearOnAdded = false};
        _surface.SadComponents.Add(controls);

        _surface.Print(0, 4, "NAME:");
        _nameTextBox = new CustomTextBox(4) { Position = (6, 4), CaretVisible = false };
        controls.Add(_nameTextBox);

        _surface.Print(0, 6, "IP:");
        _addressTextBox = new CustomTextBox(Width - 7) { Position = (6, 6) };
        controls.Add(_addressTextBox);

        Elements = [_nameTextBox, _addressTextBox];

        _placeholderSurface = new ScreenSurface(Width, 1)  { Position = (6, 6) };
        _placeholderSurface.Fill(new Color(1, 1, 1, 0.8f), Color.Transparent);
        _placeholderSurface.Print(0, 0, "127.0.0.1:25565");
        _surface.Children.Add(_placeholderSurface);

        _surface.Print(0, 10,   "[ENTER]   Join");
        _surface.Print(0, 12,   "[ESC]     Return to Main Menu");
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Center();

        _placeholderSurface.IsVisible = _addressTextBox.Text.Length <= 0;

        for (var i = 0; i < Elements.Length; i++)
        {
            var box = (CustomTextBox)Elements[i];
            box.CaretVisible = i == ElementIndex;
        }
    }

    public override void Start()
    {
        base.Start();
        _addressTextBox.Text = "";
        _addressTextBox.CaretPosition = 0;

        _nameTextBox.CaretVisible = true;
        _addressTextBox.CaretVisible = false;

        ElementIndex = 0;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Keys.Escape))
        {
            MainMenuManager.GoToMainMenu();
            return true;
        }

        return base.ProcessKeyboard(keyboard);
    }

    internal override void SubmitFinal()
    {
        var name = _nameTextBox.Text.Length > 0 ? _nameTextBox.Text : Environment.UserName;
        MainMenuManager.GameScreen.Username = name;
        MainMenuManager.GoToGameScreen();
        string ip = "127.0.0.1:25565";
        if (_addressTextBox.Text.Length > 0) ip = _addressTextBox.Text.Trim();

        Net.Connect(ip);
    }
}
