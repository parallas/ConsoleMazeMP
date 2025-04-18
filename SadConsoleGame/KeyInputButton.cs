using SadConsole.Components;
using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.Instructions;
using SadConsole.UI.Controls;

namespace SadConsoleGame;

public class KeyInputButton : Console
{
    private Keys _key;
    private string _label;
    private Action _onPress;

    private bool _isHeldDown = false;
    private string _labelKeyed;

    public KeyInputButton(int width, Keys key, string label, Action onPress) : base(width, 1)
    {
        _key = key;
        _label = label;
        _onPress = onPress;

        // Surface.DefaultBackground = Color.Transparent;
        // Surface.Fill(Color.White, Color.Transparent);

        var keyString = key.ToString();
        if (label.Contains(keyString, StringComparison.OrdinalIgnoreCase))
        {
            int index = label.IndexOf(keyString, StringComparison.OrdinalIgnoreCase);
            string preString = label.Substring(0, index);
            string postString = label.Substring(index + keyString.Length);
            _labelKeyed = $"{preString}[{keyString}]{postString}";
        }
        else
        {
            _labelKeyed = $"[{keyString}] {label}";
        }

        var drawString = new DrawString
        {
            Text = ColoredString.Parser.Parse(_labelKeyed),
            TotalTimeToPrint = TimeSpan.FromMilliseconds(50d * _labelKeyed.Length),
            RemoveOnFinished = true,
        };
        SadComponents.Add(drawString);
        // Surface.Print(0, 0, _labelKeyed);
    }

    public override bool ProcessKeyboard(Keyboard state)
    {
        if (state.IsKeyPressed(_key))
        {
            _isHeldDown = true;
            Surface.Fill(Color.Blue, Color.White);
            return true;
        }
        if (state.IsKeyReleased(_key))
        {
            _isHeldDown = false;
            Surface.Fill(Color.White, Color.Transparent);
            _onPress.Invoke();
            return true;
        }

        return base.ProcessKeyboard(state);
    }
}
