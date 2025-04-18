using SadConsole.Input;
using SadConsoleGame.Scenes;

namespace SadConsoleGame.Menus;

public class GameScreen : BaseMenu
{
    private ScreenSurface _surface;
    private TextChat _textChat;

    private string _username;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            _textChat.Username = Username;
        }
    }

    public GameScreen(ScreenSurface parentSurface, int width, int height, string username) : base(parentSurface, width, height)
    {
        _username = username;

        _surface = new ScreenSurface(width, height);
        Children.Add(_surface);

        _textChat = new TextChat(40, 14, username);
        _surface.Children.Add(_textChat);
    }

    public override void Start()
    {
        base.Start();

        _textChat.Reset();
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        int scaler = 2;
        float smallerAxisValue = Math.Min(Game1.Instance.Window.ClientBounds.Width,
            Game1.Instance.Window.ClientBounds.Height);
        if (smallerAxisValue < 700) scaler = 1;
        _surface?.Resize(
            Game1.Instance.Window.ClientBounds.Width / _surface.FontSize.X / scaler,
            Game1.Instance.Window.ClientBounds.Height / _surface.FontSize.Y / scaler,
            false
        );

        _textChat.Position = (new Point((int)(ParentSurface.Width * 0.5), (int)(ParentSurface.Height * 0.5))
                              - new Point((int)(_textChat.Width * 0.5), (int)(_textChat.Height * 0.5))) * 8;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        return _textChat.ProcessKeyboard(keyboard);
    }
}
