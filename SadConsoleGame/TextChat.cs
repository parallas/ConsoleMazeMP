using System.Text;
using System.Text.RegularExpressions;
using SadConsole.Components;
using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.Instructions;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsoleGame.Scenes;

public class TextChat : ScreenObject
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    private ScreenSurface _screenSurface;
    public Console ChatLogConsole { get; private set; }
    public TextBoxShowCaret ChatInputTextBox { get; private set; }
    private Queue<String> ChatQueue { get; set; } = new Queue<String>();
    private DrawString _drawString = new DrawString() { IsFinished = true };
    public List<String> ChatHistory { get; set; } = new List<String>();

    private static readonly Regex _usernameMatcher = _Regex.Username();

    public TextChat(int width, int height)
    {
        Width = width;
        Height = height;

        _screenSurface = new ScreenSurface(width, height)
        {
            IsVisible = true,
            Position = (0, 0),
        };
        Children.Add(_screenSurface);

        ControlHost controls = new();
        _screenSurface.SadComponents.Add(controls);

        ChatInputTextBox = new TextBoxShowCaret(width - 8)
        {
            Position = (7, height - 2),
            MaxLength = width - 9,
            CaretEffect = new Fade()
            {
                AutoReverse = true,
                DestinationBackground = new Gradient([
                    new GradientStop(Color.White, 0),
                    new GradientStop(Color.White, 0.4f),
                    new GradientStop(Color.Blue, 0.6f)
                ]),
                DestinationForeground = new Gradient([
                    new GradientStop(Color.Blue, 0),
                    new GradientStop(Color.Blue, 0.4f),
                    new GradientStop(Color.White, 0.6f)
                ]),
                FadeBackground = true,
                FadeForeground = true,
                Repeat = true,
                FadeDuration = TimeSpan.FromSeconds(0.25d),
                UseCellBackground = false,
                UseCellForeground = false,
            },
            Surface = {
                DefaultBackground = Color.Blue,
                DefaultForeground = Color.White,
            },
            FocusOnMouseClick = false,
        };

        var themeState = new ColoredGlyph(Color.White, Color.Blue);
        ChatInputTextBox.SetThemeColors(new Colors(){Appearance_ControlNormal = themeState});
        controls.Add(ChatInputTextBox);
        IsFocused = true;

        ChatLogConsole = new Console(width - 2, height - 4)
        {
            Position = (1, 1),
            IsVisible = true,
            Cursor =
            {
                IsEnabled = false,
                IsVisible = false,
            },
            Surface =
            {
                DefaultBackground = Color.Blue
            }
        };
        Children.Add(ChatLogConsole);
        DrawBox();

        void log(string message) => AddMessage(
            message
            .Replace("(SERVER): ", "serv: ")
            .Replace("(CLIENT): ", "clnt: ")
        );

        Net.Log += log;
        Net.LogInfo += log;
        Net.LogWarning += log;
        Net.LogError += log;

        Net.OnReceiveMessage += AddMessageFromUser;
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (!_drawString.IsFinished) return;
        if (ChatQueue.TryDequeue(out string? message))
        {
            ChatHistory.Add(message);
            ChatLogConsole.ShiftUp();
            ChatLogConsole.Cursor.Position = (0, ChatLogConsole.Height - 1);

            _drawString = new DrawString
            {
                Text = ColoredString.Parser.Parse($"[c:r b:Blue]{message}"),
                TotalTimeToPrint = TimeSpan.FromMilliseconds(10d * message.Length),
                RemoveOnFinished = true,
                Cursor = ChatLogConsole.Cursor,
                Position = ChatLogConsole.Cursor.Position
            };
            ChatLogConsole.SadComponents.Add(_drawString);
        }

        // ChatLogConsole.Print(0, ChatLogConsole.Height - 1, message, Color.White);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (!IsFocused) return false;

        if (keyboard.IsKeyPressed(Keys.Enter))
        {
            // Handle Enter key press
            var stringValue = ChatInputTextBox.Text.Replace('\0', ' ').Trim();
            if (stringValue.Length > 0)
            {
                AddMessageFromUser(stringValue, Environment.UserName);

                if (!stringValue.StartsWith('/'))
                    Net.SendSimpleMessage(stringValue, Environment.UserName);
            }
            ChatInputTextBox.Text = "";
            ChatInputTextBox.CaretPosition = 0;
            return true;
        }

        bool result = ChatInputTextBox.ProcessKeyboard(keyboard);
        return result;
    }

    private void DrawBox()
    {
        _screenSurface.Fill(Color.White, Color.Black);
        _screenSurface.DrawBox(new Rectangle(0, 0, Width, Height),
            ShapeParameters.CreateStyledBoxThin(Color.CornflowerBlue));
        _screenSurface.DrawLine((0, 0), (Width, 0), ICellSurface.ConnectedLineThick[0], Color.SteelBlue);
        _screenSurface.ConnectLines(ICellSurface.ConnectedLineThick);
        _screenSurface.Print(0, 0, "\u00d5");
        _screenSurface.Print(Width - 1, 0, "\u00b8");
        _screenSurface.Print(1, Height - 2, "chat: ");
    }

    public void AddMessage(string message)
    {
        ChatQueue.Enqueue(message);
    }

    public void AddMessageFromUser(string message, string user)
    {
        if(message.StartsWith('/') && ParseCommand(message))
            return;

        user = user.PadLeft(4);
        AddMessage($"{user[..4]}: {message}");
    }

    public bool ParseCommand(string message)
    {
        if(!message.StartsWith('/'))
            return false;
        message = message[1..];
        var split = message.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        switch(split[0])
        {
            case "host":
            {
                ushort port = 25565;
                if(split.Length > 1 && ushort.TryParse(split[1], out var result))
                    port = result;
                Net.StartServer(port);

                string ip = $"127.0.0.1:{port}";
                Net.Connect(ip);
                break;
            }
            case "server":
            {
                if(split.Length == 1)
                {
                    AddMessage("???");
                    break;
                }
                switch(split[1])
                {
                    case "start":
                    {
                        ushort port = 25565;
                        if(split.Length > 2 && ushort.TryParse(split[2], out var result))
                            port = result;
                        Net.StartServer(port);
                        break;
                    }
                    case "stop":
                    {
                        Net.StopServer();
                        break;
                    }
                }
                break;
            }
            case "join":
            {
                string ip = "47.208.146.216:25565";
                if(split.Length > 1)
                    ip = split[1];
                Net.Connect(ip);

                break;
            }
            case "exit":
            {
                Net.Disconnect();
                if(Net.IsServerRunning)
                    Net.StopServer();
                break;
            }
            case "cls":
            {
                ChatLogConsole.Clear();
                break;
            }
            default:
            {
                AddMessage("unknown command");
                break;
            }
        }

        return true;
    }
}
