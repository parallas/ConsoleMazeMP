using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Game = Microsoft.Xna.Framework.Game;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsoleGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    internal static Game1 Instance;

    private static RenderTarget2D _rt;

    public Game1()
    {
        Instance = this;
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Some global variables for SadConsole
        SadConsole.Game.Instance.MonoGameInstance = this;
        SadConsole.Host.Global.GraphicsDeviceManager = _graphics;

        // Initialize the SadConsole engine
        SadConsole.Host.Global.SadConsoleComponent = new SadConsole.Host.SadConsoleGameComponent(this);
        Components.Add(SadConsole.Host.Global.SadConsoleComponent);

        _rt = new(GraphicsDevice, GameSettings.GAME_WIDTH * 8, GameSettings.GAME_HEIGHT * 8);

        // Window.ClientSizeChanged += delegate {
        //     _rt = new(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);
        // };

        RtScreen.Init();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Do your offscreen drawing (the SadConsole component gets drawn here)
        base.Draw(gameTime);

        // Clear the graphics device
        // GraphicsDevice.SetRenderTarget(_rt);
        GraphicsDevice.Clear(Color.Black);

        RtScreen.DrawWithRtOnScreen(
            _rt,
            _graphics,
            _spriteBatch,
            null!,
            null!,
            Color.White,
            () => {
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                _spriteBatch.Draw(SadConsole.Host.Global.RenderOutput, Vector2.Zero, Color.White);
                _spriteBatch.End();
            }
        );
    }
}
