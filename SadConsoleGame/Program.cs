using SadConsole.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        Settings.WindowTitle = "My SadConsole Game";

        Builder gameStartup = new Builder()
            .SetScreenSize(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
            .SetStartingScreen<SadConsoleGame.Scenes.RootScreen>()
            .ConfigureFonts(ConfigureFonts);

        Game.Create(gameStartup);
        Game.Instance.Run();
        Game.Instance.Dispose();
    }

    private static void ConfigureFonts(FontConfig config, GameHost host)
    {
        config.UseCustomFont("data/fonts/8x8.font");
    }
}
