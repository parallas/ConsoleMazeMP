using System.Diagnostics;
using Riptide;
using Riptide.Utils;
using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsoleGame.Scenes;

class RootScreen : ScreenObject
{
    private ScreenSurface? _mainSurface;
    private TextChat _textChat;

    public RootScreen()
    {
        _mainSurface = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
        SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.None;
        Game1.Instance.Window.ClientSizeChanged += Game_WindowResized;

        FillBackground();

        Children.Add(_mainSurface);

        _textChat = new TextChat(40, 14)
        {
            Position = new Point(2, _mainSurface.Height - 14 - 2) * 8,
            // IsFocused = true,
        };
        _mainSurface.Children.Add(_textChat);

        RefreshScreen();

        Net.Init();
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Net.Update();
    }

    private void Game_WindowResized(object? sender, EventArgs e)
    {
        RefreshScreen();
    }

    public void RefreshScreen()
    {
        int scaler = 2;
        float smallerAxisValue = Math.Min(Game1.Instance.Window.ClientBounds.Width,
            Game1.Instance.Window.ClientBounds.Height);
        if (smallerAxisValue < 700) scaler = 1;

        _mainSurface?.Resize(
            Game1.Instance.Window.ClientBounds.Width / _mainSurface.FontSize.X / scaler,
            Game1.Instance.Window.ClientBounds.Height / _mainSurface.FontSize.Y / scaler,
            false
        );

        SadConsole.Host.Global.RecreateRenderOutputHandler(
            _mainSurface.Width * _mainSurface.FontSize.X,
            _mainSurface.Height * _mainSurface.FontSize.Y
        );

        FillBackground();

        _textChat.Position = (new Point((int)(_mainSurface.Width * 0.5), (int)(_mainSurface.Height * 0.5))
                              - new Point((int)(_textChat.Width * 0.5), (int)(_textChat.Height * 0.5))) * 8;
    }

    private void FillBackground()
    {
        // _mainSurface.FillWithRandomGarbage(_mainSurface.Font);

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
