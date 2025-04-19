using SadConsole.Input;
using SadConsole.UI;

namespace SadConsoleGame.Menus;

public class HostGame : SequencedMenu
{
    private ScreenSurface _surface;

    private CustomTextBox _nameTextBox;
    private CustomTextBox _portTextBox;
    private ScreenSurface _placeholderSurface;

    public HostGame(ScreenSurface surface) : base(surface, 32, 16)
    {
        _surface = new ScreenSurface(Width, Height);
        _surface.Fill(Color.White, Color.Transparent, 0);
        Children.Add(_surface);

        _surface.Print(0, 0, "Host Game");

        ControlHost controls = new() {ClearOnAdded = false};
        _surface.SadComponents.Add(controls);

        _surface.Print(0, 4, "NAME:");
        _nameTextBox = new CustomTextBox(4) { Position = (6, 4), CaretVisible = true };
        controls.Add(_nameTextBox);

        _surface.Print(0, 6, "PORT:");
        _portTextBox = new CustomTextBox(5, true) { Position = (6, 6), CaretVisible = false };
        controls.Add(_portTextBox);

        Elements = [_nameTextBox, _portTextBox];

        _placeholderSurface = new ScreenSurface(5, 1)  { Position = (6, 6) };
        _placeholderSurface.Fill(new Color(1, 1, 1, 0.8f), Color.Transparent);
        _placeholderSurface.Print(0, 0, "25565");
        _surface.Children.Add(_placeholderSurface);

        _surface.Print(0, 10,    "[ENTER]   Host");
        _surface.Print(0, 12,   "[ESC]     Return to Main Menu");
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Center();

        _placeholderSurface.IsVisible = _portTextBox.Text.Length <= 0;

        for (var i = 0; i < Elements.Length; i++)
        {
            var box = (CustomTextBox)Elements[i];
            box.CaretVisible = i == ElementIndex;
        }
    }

    public override void Start()
    {
        base.Start();
        _portTextBox.Text = "";
        _portTextBox.CaretPosition = 0;
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

        ushort port = 25565;
        if (_portTextBox.Text.Length > 0) port = ushort.Parse(_portTextBox.Text);
        Net.StartServer(port);

        string ip = $"127.0.0.1:{port}";
        Net.Connect(ip);
    }
}
