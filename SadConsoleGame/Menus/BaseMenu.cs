namespace SadConsoleGame.Menus;

public abstract class BaseMenu(ScreenSurface parentSurface, int width, int height) : ScreenObject
{
    public ScreenSurface ParentSurface { get; private set; } = parentSurface;
    public int Width { get; protected set; } = width;
    public int Height { get; protected set; } = height;

    public void Center()
    {
        Position = (new Point((int)(ParentSurface.Width * 0.5), (int)(ParentSurface.Height * 0.5))
                    - new Point((int)(Width * 0.5),
                        (int)(Height * 0.5))) * 8;
    }
}
