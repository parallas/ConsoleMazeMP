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

        Window.AllowUserResizing = true;

        // Initialize the SadConsole engine
        SadConsole.Host.Global.SadConsoleComponent = new SadConsole.Host.SadConsoleGameComponent(this);
        Components.Add(SadConsole.Host.Global.SadConsoleComponent);

        _rt = new(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);

        RtScreen.Init();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here

        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (_rt.Width != SadConsole.Host.Global.RenderOutput.Width || _rt.Height != SadConsole.Host.Global.RenderOutput.Height)
        {
            _rt = new(GraphicsDevice, SadConsole.Host.Global.RenderOutput.Width, SadConsole.Host.Global.RenderOutput.Height);
        }

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
