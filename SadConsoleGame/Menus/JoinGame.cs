using SadConsole.Input;
using SadConsole.UI;

namespace SadConsoleGame.Menus;

public class JoinGame : BaseMenu
{
    private ScreenSurface _surface;

    private CustomTextBox _addressTextBox;
    private CustomTextBox _nameTextBox;

    private ScreenSurface _placeholderSurface;

    private CustomTextBox[] _textBoxes;
    private int _focusedTextBoxIndex = 0;

    public JoinGame(ScreenSurface surface) : base(surface, 32, 16)
    {
        _surface = new ScreenSurface(Width, Height);
        _surface.Fill(Color.White, Color.Transparent, 0);
        Children.Add(_surface);

        _surface.Print(0, 0, "Join Game");

        ControlHost controls = new() {ClearOnAdded = false};
        _surface.SadComponents.Add(controls);

        _surface.Print(0, 4, "IP:");
        _addressTextBox = new CustomTextBox(Width - 7) { Position = (6, 4) };
        controls.Add(_addressTextBox);

        _surface.Print(0, 6, "NAME:");
        _nameTextBox = new CustomTextBox(4) { Position = (6, 6), CaretVisible = false };
        controls.Add(_nameTextBox);

        _textBoxes = [_addressTextBox, _nameTextBox];

        _placeholderSurface = new ScreenSurface(Width, 1)  { Position = (6, 4) };
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
    }

    public override void Start()
    {
        base.Start();
        _addressTextBox.Text = "";
        _addressTextBox.CaretPosition = 0;

        _addressTextBox.CaretVisible = true;
        _nameTextBox.CaretVisible = false;

        _focusedTextBoxIndex = 0;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyReleased(Keys.Escape))
        {
            MainMenuManager.GoToMainMenu();
            return true;
        }

        if (keyboard.IsKeyPressed(Keys.Tab))
        {
            var dir = keyboard.IsKeyDown(Keys.LeftShift) ? -1 : 1;
            SetFocusedTextBoxIndex(_focusedTextBoxIndex + dir);
            return true;
        }

        if (keyboard.IsKeyReleased(Keys.Enter))
        {
            switch (_focusedTextBoxIndex)
            {
                case 0:
                    SetFocusedTextBoxIndex(_focusedTextBoxIndex + 1);
                    break;
                case 1:
                    var name = _nameTextBox.Text.Length > 0 ? _nameTextBox.Text : Environment.UserName;
                    MainMenuManager.GameScreen.Username = name;
                    MainMenuManager.GoToGameScreen();
                    string ip = $"127.0.0.1:25565";
                    if (_addressTextBox.Text.Length > 0) ip = _addressTextBox.Text.Trim();

                    Net.Connect(ip);
                    break;
            }
            return true;
        }

        return _textBoxes[_focusedTextBoxIndex].ProcessKeyboard(keyboard);
    }

    private void SetFocusedTextBoxIndex(int index)
    {
        _textBoxes[_focusedTextBoxIndex].CaretVisible = false;
        _focusedTextBoxIndex = index;
        if (_focusedTextBoxIndex < 0) _focusedTextBoxIndex = _textBoxes.Length - 1;
        if (_focusedTextBoxIndex >= _textBoxes.Length) _focusedTextBoxIndex = 0;
        _textBoxes[_focusedTextBoxIndex].CaretVisible = true;
    }
}
