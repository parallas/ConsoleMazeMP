using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsoleGame;

public static class ContentLoader
{
    private static readonly string _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

    private static readonly List<string> _missingPaths = [];

    private static ContentManager _xnaLoader = null!;
    private static GraphicsDevice _graphicsDevice = null!;

    internal static void Initialize(ContentManager shaderLoader, GraphicsDevice graphicsDevice)
    {
        if(_xnaLoader is not null)
            throw new InvalidOperationException("attempted to initialize ContentLoader after it has already been initialized");

        _xnaLoader = shaderLoader;
        _graphicsDevice = graphicsDevice;
    }

    public static T? Load<T>(string path) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string filePath = Path.Combine(_dataPath, path);
        if(Check<T,Effect>() || Check<T,SpriteFont>())
        {
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _xnaLoader.RootDirectory, path);
        }

        lock(_missingPaths)
            if(_missingPaths.Contains(path))
                return default;

        try
        {
            var ret = LoadInternal<T>(filePath);
            if(ret is default(T))
                lock(_missingPaths)
                    _missingPaths.Add(path);
            return ret;
        }
        catch(Exception e)
        {
            System.Console.Error.WriteLine($"data file {path} could not be loaded: {e}");

            lock(_missingPaths)
                _missingPaths.Add(path);

            return default;
        }
    }

    private static T? LoadInternal<T>(string path) where T : class
    {
        if(!File.Exists(path) && !Check<T,Effect>() && !Check<T,SpriteFont>())
            throw new FileNotFoundException($"file does not exist or could not be found");

        if(Check<T,Effect>() || Check<T,SpriteFont>())
        {
            lock(_xnaLoader)
                return _xnaLoader.Load<T>(Path.ChangeExtension(path, null));
        }
        else if (Check<T,Texture2D>())
        {
            lock(_graphicsDevice)
                return Texture2D.FromFile(_graphicsDevice, path) as T;
        }

        throw new Exception($"the target type {typeof(T).FullName} is not loadable from a file");
    }

    private static bool Check<T, K>() where T : class where K : class
        => typeof(T).IsAssignableTo(typeof(K));
}
