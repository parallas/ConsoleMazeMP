using SadConsoleGame.Menus;

namespace SadConsoleGame;

public static class MainMenuManager
{
    public static MainMenu MainMenu;
    public static HostGame HostGame;

    private static readonly List<BaseMenu> Menus = new List<BaseMenu>();

    public static void EnableMenu(BaseMenu menu, bool isolated = false)
    {
        if (isolated)
        {
            DisableAllMenus();
        }

        menu.IsEnabled = true;
        menu.IsVisible = true;
    }

    public static void DisableMenu(BaseMenu menu)
    {
        menu.IsEnabled = false;
        menu.IsVisible = false;
    }

    public static void DisableAllMenus()
    {
        DisableMenu(MainMenu);
        DisableMenu(HostGame);
    }

    public static void GoToMainMenu()
    {
        EnableMenu(MainMenu, true);
    }

    public static void GoToHostGame()
    {
        EnableMenu(HostGame, true);
    }
}
