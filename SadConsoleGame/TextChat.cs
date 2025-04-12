using System.Text;
using SadConsole.Components;
using SadConsole.Input;

namespace SadConsoleGame.Scenes;

public class TextChat : ScreenObject
{
    private ScreenSurface _screenSurface;
    public Console ChatLogConsole { get; private set; }
    public Console ChatInputConsole { get; private set; }
    public List<String> ChatHistory { get; set; } = new List<String>();

    public TextChat(int width, int height)
    {
        _screenSurface = new ScreenSurface(width, height)
        {
            IsVisible = true,
            Position = (0, 0),
        };
        Children.Add(_screenSurface);
        _screenSurface.DrawBox(new Rectangle(0, 0, width, height),
            ShapeParameters.CreateStyledBoxThin(Color.CornflowerBlue));
        _screenSurface.Print(2, height - 2, "chat: ");

        ChatInputConsole = new Console(width - 10, 1)
        {
            Position = (8, height - 2),
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

        ChatLogConsole = new Console(width - 4, height - 4)
        {
            Position = (2, 1),
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
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (!IsFocused) return false;

        if (keyboard.IsKeyPressed(Keys.Enter))
        {
            // Handle Enter key press
            var stringValue = ChatInputConsole.GetString(0, ChatLogConsole.Width);
            if (stringValue.Trim().Length > 0)
            {
                ChatHistory.Add($"{1111}: {stringValue}");

                ChatLogConsole.ShiftUp();
                ChatLogConsole.Print(0, ChatLogConsole.Height - 1, ChatHistory.Last(), Color.White);
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
}
