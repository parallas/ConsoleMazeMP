using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsoleGame.Menus;

public class CustomTextBox : TextBoxShowCaret
{
    private bool _numbersOnly = false;
    public CustomTextBox(int width, bool numbersOnly = false) : base(width + 1)
    {
        _numbersOnly = numbersOnly;
        MaxLength = width;
        CaretEffect = new Fade
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
        };
        FocusOnMouseClick = false;

        var themeState = new ColoredGlyph(Color.White, Color.Blue);
        SetThemeColors(new Colors()
        {
            Appearance_ControlNormal = themeState,
            Appearance_ControlDisabled = themeState,
            Appearance_ControlSelected = themeState,
            Appearance_ControlFocused = themeState,
            Appearance_ControlOver = themeState
        });
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (_numbersOnly)
        {
            var caretPosition = CaretPosition;
            var result = base.ProcessKeyboard(keyboard);
            var filteredString = new string(Text.Where(char.IsDigit).ToArray());
            if (filteredString != Text)
            {
                Text = filteredString;
                CaretPosition = caretPosition;
                return false;
            }
            return result;
        }

        return base.ProcessKeyboard(keyboard);
    }
}
