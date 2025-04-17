using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.UI.Controls;

namespace SadConsoleGame;

public class KeyInputButton : ControlBase
{
    private Keys _key;
    private string _label;
    private Action _onPress;

    private bool _isHeldDown = false;
    public KeyInputButton(int width, Keys key, string label, Action onPress) : base(width, 1)
    {
        _key = key;
        _label = label;
        _onPress = onPress;

        Surface.DefaultBackground = Color.Transparent;

        Surface.Fill(Color.White, Color.Transparent);

        var keyString = key.ToString();
        if (label.Contains(keyString, StringComparison.OrdinalIgnoreCase))
        {
            int index = label.IndexOf(keyString, StringComparison.OrdinalIgnoreCase);
            string preString = label.Substring(0, index);
            string postString = label.Substring(index + keyString.Length);
            string labelKeyed = $"{preString}[{keyString}]{postString}";
            Surface.Print(0, 0, labelKeyed);
        }
        else
        {
            Surface.Print(0, 0, $"[{keyString}] {label}");
        }
    }

    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (_isHeldDown)
        {
            Surface.Fill(Color.Blue, Color.White);
        }
        else
        {
            Surface.Fill(Color.White, Color.Transparent);
        }
    }

    public override bool ProcessKeyboard(Keyboard state)
    {
        if (state.IsKeyPressed(_key))
        {
            _isHeldDown = true;
            UpdateAndRedraw(TimeSpan.Zero);
            return true;
        }
        if (state.IsKeyReleased(_key))
        {
            _isHeldDown = false;
            UpdateAndRedraw(TimeSpan.Zero);
            _onPress.Invoke();
            return true;
        }

        return base.ProcessKeyboard(state);
    }
}
