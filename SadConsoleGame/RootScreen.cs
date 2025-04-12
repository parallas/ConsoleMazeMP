using System.Diagnostics;
using Riptide;
using Riptide.Utils;
using SadConsole.Input;

namespace SadConsoleGame.Scenes;

class RootScreen : ScreenObject
{
    private ScreenSurface? _mainSurface;
    private TextChat _textChat;

    public RootScreen()
    {
        // Create a surface that's the same size as the screen.

        // SadConsole.Settings.ResizeMode = Settings.WindowResizeOptions.None;

        // var xScale = SadConsole.Host.Global.GraphicsDeviceManager.PreferredBackBufferWidth / 8;
        // var yScale = SadConsole.Host.Global.GraphicsDeviceManager.PreferredBackBufferHeight / 8;
        // _mainSurface = new ScreenSurface(xScale, yScale);
        // _mainSurface.FontSize = _mainSurface.Font.GetFontSize(IFont.Sizes.Four);
        // SadConsole.Settings.ResizeMode = Settings.WindowResizeOptions.None;

        _mainSurface = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
        SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.None;
        Game1.Instance.Window.ClientSizeChanged += Game_WindowResized;

        // Fill the surface with random characters and colors
        // _mainSurface.FillWithRandomGarbage(_mainSurface.Font);
        FillBackground();
        
        // Create a rectangle box that has a violet foreground and black background.
        // Characters are reset to 0 and mirroring is set to none. FillWithRandomGarbage will
        // select random characters and mirroring, so this resets it within the box.
        // _mainSurface.Fill(new Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 1, Mirror.None);
        
        // Print some text at (4, 4) using the foreground and background already there (violet and black)
        // _mainSurface.Print(4, 4, "Hello from SadConsole");

        // Add _mainSurface as a child object of this one. This object, RootScreen, is a simple object
        // and doesn't display anything itself. Since _mainSurface is going to be a child of it, _mainSurface
        // will be displayed.
        Children.Add(_mainSurface);

        _textChat = new TextChat(40, 14)
        {
            Position = (3, 2),
            IsFocused = true,
        };

        _mainSurface.Children.Add(_textChat);

        Net.Init();
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Net.Update();
    }

    void Game_WindowResized(object? sender, EventArgs e)
    {
        SadConsole.Host.Global.GraphicsDeviceManager.PreferredBackBufferWidth =
            Game1.Instance.Window.ClientBounds.Width;
        SadConsole.Host.Global.GraphicsDeviceManager.PreferredBackBufferHeight =
            Game1.Instance.Window.ClientBounds.Height;

        SadConsole.Host.Global.GraphicsDeviceManager.ApplyChanges();

        _mainSurface?.Resize(
            Game1.Instance.Window.ClientBounds.Width / _mainSurface.FontSize.X / 2,
            Game1.Instance.Window.ClientBounds.Height / _mainSurface.FontSize.Y / 2,
            false
        );

        SadConsole.Host.Global.RecreateRenderOutputHandler(
            _mainSurface.Width * _mainSurface.FontSize.X,
            _mainSurface.Height * _mainSurface.FontSize.Y
        );

        FillBackground();
    }

    void FillBackground()
    {
        Color[] colors = new[] { Color.DarkBlue, Color.Black };
        float[] colorStops = new[] { 0f, 1f };

        Algorithms.GradientFill(_mainSurface.FontSize,
            _mainSurface.Surface.Area.Center,
            _mainSurface.Surface.Height,
            0,
            _mainSurface.Surface.Area,
            new Gradient(colors, colorStops),
            (x, y, color) => _mainSurface.Surface[x, y].Background = color);
    }

}
