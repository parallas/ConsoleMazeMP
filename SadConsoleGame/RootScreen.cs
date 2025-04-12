using System.Diagnostics;
using Riptide;
using Riptide.Utils;
using SadConsole.Input;

namespace SadConsoleGame.Scenes;

class RootScreen : ScreenObject
{
    private ScreenSurface _mainSurface;
    private TextChat _textChat;

    private static Server _server;
    private static Client _client;

    public RootScreen()
    {
        // Create a surface that's the same size as the screen.
        // _mainSurface = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

        // Fill the surface with random characters and colors
        // _mainSurface.FillWithRandomGarbage(_mainSurface.Font);
        
        // Create a rectangle box that has a violet foreground and black background.
        // Characters are reset to 0 and mirroring is set to none. FillWithRandomGarbage will
        // select random characters and mirroring, so this resets it within the box.
        // _mainSurface.Fill(new Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 1, Mirror.None);
        
        // Print some text at (4, 4) using the foreground and background already there (violet and black)
        // _mainSurface.Print(4, 4, "Hello from SadConsole");

        // Add _mainSurface as a child object of this one. This object, RootScreen, is a simple object
        // and doesn't display anything itself. Since _mainSurface is going to be a child of it, _mainSurface
        // will be displayed.
        // Children.Add(_mainSurface);

        _textChat = new TextChat(42, 14)
        {
            Position = (3, 2),
            IsFocused = true,
        };

        Children.Add(_textChat);

        RiptideLogger.Initialize(Log, LogInfo, LogWarning, LogError, false);

        _server = new Server();
        _client = new Client(Environment.MachineName);
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        _server.Update();
        _client.Update();
    }

    public enum ClientToServer
    {
        None,
        SendMessage,
    }

    public enum ServerToClient
    {
        None,
        SendMessage,
    }

    // public override bool ProcessKeyboard(Keyboard keyboard)
    // {
    //     if (keyboard.IsKeyPressed(Keys.S))
    //     {
    //         if (_server.IsRunning)
    //         {
    //             _server.Stop();
    //             return true;
    //         }
    //         _server.Start(25565, 4);
    //     }
    //     if (keyboard.IsKeyPressed(Keys.C))
    //     {
    //         if (!_client.IsNotConnected)
    //         {
    //             _client.Disconnect();
    //             return true;
    //         }
    //         _client.Connect("47.208.146.216:25565");
    //     }
    //
    //     if (!_client.IsConnected) return false;
    //     if (keyboard.IsKeyPressed(Keys.Space))
    //     {
    //         SendSimpleMessage($"Hello from the client! {_client.LogName}");
    //     }
    //     return base.ProcessKeyboard(keyboard);
    // }

    public void SendSimpleMessage(String stringToSend)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (int)ClientToServer.SendMessage);
        message.AddString(stringToSend);

        _client.Send(message); // Sends the message to the server
        // _server.Send(message, <toClientId>); // Sends the message to a specific client
        // _server.SendToAll(message); // Sends the message to all connected clients
        // _server.SendToAll(message, <toClientId>); // Sends the message to all connected clients except the specified one
    }

    [MessageHandler((ushort)ClientToServer.SendMessage)]
    private static void HandleSimpleMessageServerRpc(ushort clientId, Message message)
    {
        String messageString = message.GetString();
        Log($"Received message from client {clientId}: {messageString}");

        _server.SendToAll(message);
    }

    [MessageHandler((ushort)ServerToClient.SendMessage)]
    private static void HandleSimpleMessageClientRpc(Message message)
    {
        String messageString = message.GetString();
        Log($"Received message from server: {messageString}");
    }

    public static void Log(string message)
    {
        Debug.WriteLine(message);
    }
    private void LogInfo(string message)
    {
        Debug.WriteLine($"Info: {message}");
    }
    private void LogWarning(string message)
    {
        Debug.WriteLine($"Warning: {message}");
    }
    private void LogError(string message)
    {
        Debug.WriteLine($"Error: {message}");
    }
}
