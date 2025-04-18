namespace SadConsoleGame.Menus;

public class HostGame : BaseMenu
{
    private ScreenSurface _surface;
    public HostGame(ScreenSurface surface) : base(surface, 32, 16)
    {
        _surface = new ScreenSurface(Width, Height);
        _surface.Fill(Color.White, Color.Transparent, 0);
        Children.Add(_surface);

        _surface.Print(0, 0, "Host Game");
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Center();
    }
}
