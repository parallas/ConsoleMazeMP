using SadConsole.Effects;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsoleGame;

public class TextBoxShowCaret : TextBox
{
    private int _oldCaretPosition;
    private ControlStates _oldState = ControlStates.Disabled;
    private string _editingText = string.Empty;

    private bool _caretVisible = true;
    public bool CaretVisible
    {
        get => _caretVisible;
        set
        {
            if (_caretVisible == value) return;
            _caretVisible = value;
            IsDirty = true;
            _oldState = ControlStates.Disabled;
            UpdateAndRedraw(TimeSpan.Zero);
        }
    }

    public TextBoxShowCaret(int width) : base(width)
    {
        // CaretEffect is now created via the base, add it to the surface
        Surface.SetEffect(CaretPosition, 0, CaretEffect);
    }

    protected override ICellSurface CreateControlSurface()
    {
        var surface = new CellSurface(Width, Height)
        {
            DefaultBackground = SadRogue.Primitives.Color.Black
        };
        surface.Clear();

        // This doesn't work at ctor time because CaretEffect is created after this method is called
        surface.SetEffect(CaretPosition - LeftDrawOffset, 0, CaretEffect);

        return surface;
    }

    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (Surface.Effects.Count != 0)
        {
            Surface.Effects.UpdateEffects(time);
            IsDirty = IsDirty || Surface.IsDirty;
        }

        if (!IsDirty) return;

        Colors colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        bool isFocusedSameAsBack = ThemeState.Focused.Background == colors.ControlHostBackground;

        ThemeState.Normal.Background = colors.GetOffColor(ThemeState.Normal.Background, colors.ControlHostBackground);
        ThemeState.MouseOver.Background = colors.GetOffColor(ThemeState.MouseOver.Background, colors.ControlHostBackground);
        ThemeState.MouseDown.Background = colors.GetOffColor(ThemeState.MouseDown.Background, colors.ControlHostBackground);
        ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, colors.ControlHostBackground);

        if (isFocusedSameAsBack)
            ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, ThemeState.Focused.Background);

        ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, ThemeState.Normal.Background);

        ColoredGlyphBase appearance = ThemeState.GetStateAppearance(State);

        if (_oldCaretPosition != CaretPosition || _oldState != State || _editingText != Text)
        {
            Surface.Fill(appearance.Foreground, appearance.Background, 0, Mirror.None);

            if (Mask == null)
                Surface.Print(0, 0, Text.Substring(LeftDrawOffset));
            else
                Surface.Print(0, 0, Text.Substring(LeftDrawOffset).Masked(Mask.Value));

            _oldCaretPosition = CaretPosition;
            _oldState = State;
            _editingText = Text;
            CaretEffect.Restart();

            if (CaretVisible) Surface.SetEffect(CaretPosition - LeftDrawOffset, 0, CaretEffect);
        }
    }
}
