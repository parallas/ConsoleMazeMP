using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace SadConsoleGame;

public class TestFakePerspective
{
    private readonly List<RenderObject> _renderObjects;

    private Vector3 _camPos = new Vector3(0, 0, 8);
    private float _counter;

    private const float FovConst = 0.03f; // Larger value increases the perspective effect

    public TestFakePerspective()
    {
        int tileDistance = 8;
        _renderObjects = new List<RenderObject>(20 * 20);
        for (int x = 0; x < 20; x++)
        for (int y = 0; y < 20; y++)
        {
            var pos = new Vector2(x, y) * tileDistance - Vector2.One * 10 * tileDistance;
            var pos3 = new Vector3(pos.X, pos.Y, Random.Shared.Next(0, 3));
            _renderObjects.Add(new RenderObject
            {
                Position = pos3
            });
        }
    }

    public void Update(float deltaTime)
    {
        _counter += deltaTime;

        var keyboardState = Keyboard.GetState();

        var moveInput = new Vector2(
            (keyboardState.IsKeyDown(Keys.Right) ? 1 : 0) - (keyboardState.IsKeyDown(Keys.Left) ? 1 : 0),
            (keyboardState.IsKeyDown(Keys.Down) ? 1 : 0) - (keyboardState.IsKeyDown(Keys.Up) ? 1 : 0)
        );
        _camPos += new Vector3(moveInput.X, moveInput.Y, 0) * 10 * 2 * deltaTime;

        var zoomInput = (keyboardState.IsKeyDown(Keys.OemCloseBrackets) ? 1 : 0) -
                        (keyboardState.IsKeyDown(Keys.OemOpenBrackets) ? 1 : 0);
        _camPos.Z += zoomInput * 3f * deltaTime;
        _camPos.Z = Math.Clamp(_camPos.Z, 3, 15);

        // var camPos2d = new Vector2(MathF.Sin(_counter) * 64f, MathF.Cos(_counter) * 64f) * deltaTime * 10f;
        // var camPosZ = (MathF.Cos(_counter * 1.0f + 32.12f) + 1) / 2 + 8;
        // _camPos = new Vector3(camPos2d.X, camPos2d.Y, camPosZ);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 screenSize)
    {
        var tex = ContentLoader.Load<Texture2D>("graphics/test.png");

        _renderObjects.Sort(((o1, o2) => o1.Position.Z < o2.Position.Z ? -1 : 1));
        foreach (var renderObject in _renderObjects)
        {
            var calcScale = CalculatePerspectiveAmount(renderObject.Position.Z);
            if (calcScale < 0) continue;
            var opacity = 1f;
            if (calcScale > 10f && renderObject.Position.Z >= 1f) opacity = 1 - (calcScale - 10f) / 2f;
            if (opacity < 0) continue;
            var pos = TransformPosition(renderObject.Position, screenSize);
            spriteBatch.Draw(
                tex,
                pos,
                null,
                Color.Lerp(Color.MediumBlue, Color.LightBlue, calcScale / 10f) * opacity,
                0f,
                Vector2.One * 4f,
                Vector2.One * TransformScale(renderObject.Position.Z),
                SpriteEffects.None,
                0
            );
        }

        var characterCalcScale = CalculatePerspectiveAmount(1);
        var characterPos = TransformPosition(new Vector3(_camPos.X, _camPos.Y, 1), screenSize);
        spriteBatch.Draw(
            tex,
            characterPos,
            null,
            Color.CornflowerBlue,
            0f,
            Vector2.One * 4f,
            Vector2.One * TransformScale(1) * 0.5f,
            SpriteEffects.None,
            0
        );
    }

    private Vector2 TransformPosition(Vector3 position, Vector2 screenSize)
    {
        var camPos = _camPos;
        // var camScale = 1f / (position.Z + 1) + camPos.Z;
        var camScale = CalculatePerspectiveAmount(position.Z);

        var screenPos = new Vector2(
            (position.X - camPos.X) * camScale + screenSize.X * 0.5f,
            (position.Y - camPos.Y) * camScale + screenSize.Y * 0.5f
        );

        return screenPos;
    }

    private float TransformScale(float depth)
    {
        var camPos = _camPos;
        // var camScale = 1f / (depth + 1) + camPos.Z;
        var camScale = CalculatePerspectiveAmount(depth);

        return camScale;
    }

    private float CalculatePerspectiveAmount(float posZ)
    {
        var camScale = 1f / (_camPos.Z - posZ) / FovConst;

        return camScale;
    }

    private struct RenderObject
    {
        public Vector3 Position;
    }
}
