using SadConsoleGame.Menus;

namespace SadConsoleGame;

public static class MainMenuManager
{
    public static MainMenu MainMenu;
    public static HostGame HostGame;
    public static JoinGame JoinGame;
    public static GameScreen GameScreen;

    private static readonly List<BaseMenu> Menus = new List<BaseMenu>();

    public static void EnableMenu(BaseMenu menu, bool isolated = false)
    {
        if (isolated)
        {
            DisableAllMenus();
        }

        menu.IsEnabled = true;
        menu.IsVisible = true;
        menu.IsFocused = true;

        menu.Start();
    }

    public static void DisableMenu(BaseMenu menu)
    {
        menu.IsEnabled = false;
        menu.IsVisible = false;
        menu.IsFocused = false;
    }

    public static void DisableAllMenus()
    {
        DisableMenu(MainMenu);
        DisableMenu(HostGame);
        DisableMenu(GameScreen);
        DisableMenu(JoinGame);
    }

    public static void GoToMainMenu()
    {
        EnableMenu(MainMenu, true);
    }

    public static void GoToHostGame()
    {
        EnableMenu(HostGame, true);
    }

    public static void GoToGameScreen()
    {
        EnableMenu(GameScreen, true);
    }

    public static void GoToJoinGame()
    {
        EnableMenu(JoinGame, true);
    }
}
