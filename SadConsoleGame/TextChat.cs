using System.Text;
using System.Text.RegularExpressions;
using SadConsole.Components;
using SadConsole.Input;

namespace SadConsoleGame.Scenes;

public class TextChat : ScreenObject
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    private ScreenSurface _screenSurface;
    public Console ChatLogConsole { get; private set; }
    public Console ChatInputConsole { get; private set; }
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
        _screenSurface.Fill(Color.White, Color.Black);
        _screenSurface.DrawBox(new Rectangle(0, 0, width, height),
            ShapeParameters.CreateStyledBoxThin(Color.CornflowerBlue));
        _screenSurface.DrawLine((0, 0), (width, 0), ICellSurface.ConnectedLineThick[0], Color.SteelBlue);
        _screenSurface.ConnectLines(ICellSurface.ConnectedLineThick);
        _screenSurface.Print(0, 0, "\u00d5");
        _screenSurface.Print(width - 1, 0, "\u00b8");
        _screenSurface.Print(1, height - 2, "chat: ");

        ChatInputConsole = new Console(width - 8, 1)
        {
            Position = (7, height - 2),
            Cursor =
            {
                IsEnabled = true,
                IsVisible = true,
                MouseClickReposition = true,
            },
            Surface =
            {
                DefaultBackground = Color.Blue
            }
        };

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

        Children.Add(ChatInputConsole);
        Children.Add(ChatLogConsole);

        IsFocused = true;

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

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (!IsFocused) return false;

        if (keyboard.IsKeyPressed(Keys.Enter))
        {
            // Handle Enter key press
            var stringValue = ChatInputConsole.GetString(0, ChatLogConsole.Width).Replace('\0', ' ').Trim();
            if (stringValue.Length > 0)
            {
                AddMessageFromUser(stringValue, Environment.UserName);

                if (!stringValue.StartsWith('/'))
                    Net.SendSimpleMessage(stringValue, Environment.UserName);
            }
            ChatInputConsole.Clear();
            ChatInputConsole.Cursor.Position = Point.Zero;
            return true;
        }

        if (ChatInputConsole.Cursor.Position.X >= ChatInputConsole.Width - 1)
        {
            if (keyboard.IsKeyPressed(Keys.Left))
            {
                ChatInputConsole.Cursor.Position -= (1, 0);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.Back))
            {
                // Handle Backspace key press
                if (ChatInputConsole.Cursor.Position.X > 0)
                {
                    ChatInputConsole.Cursor.Position = new Point(ChatInputConsole.Cursor.Position.X - 1, 0);
                    ChatInputConsole.Print(ChatInputConsole.Cursor.Position.X, ChatInputConsole.Cursor.Position.Y, " ");
                }
                return true;
            }

            return false;
        }

        bool result = ChatInputConsole.ProcessKeyboard(keyboard);
        return result;
    }

    public void AddMessage(string message)
    {
        ChatHistory.Add(message);
        ChatLogConsole.ShiftUp();
        ChatLogConsole.Print(0, ChatLogConsole.Height - 1, message, Color.White);
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
                if(!Net.Connect(ip))
                    AddMessage("already connected");
                else
                    AddMessage("connecting...");
                break;
            }
            case "exit":
            {
                if(Net.Disconnect())
                    AddMessage("leaving...");
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
