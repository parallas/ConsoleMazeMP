using System.Diagnostics;
using Riptide;
using Riptide.Utils;

namespace SadConsoleGame;

public static class Net
{
    private static Server _server;
    private static Client _client;

    public static bool IsServerRunning => _server?.IsRunning ?? false;

    public static void Init()
    {
        _server = new();
        _client = new();

        RiptideLogger.Initialize(ConsoleLog, ConsoleLogInfo, ConsoleLogWarning, ConsoleLogError, false);
    }

    public static void Update()
    {
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

    [MessageHandler((ushort)ClientToServer.SendMessage)]
    private static void HandleSimpleMessageServerRpc(ushort clientId, Message message)
    {
        String messageString = message.GetString();
        ConsoleLog($"Received message from client {clientId}: {messageString}");

        _server.SendToAll(message);
    }

    [MessageHandler((ushort)ServerToClient.SendMessage)]
    private static void HandleSimpleMessageClientRpc(Message message)
    {
        string messageString = message.GetString();
        ConsoleLog($"Received message from server: {messageString}");
    }

    public static event Action<string>? Log;
    public static event Action<string>? LogInfo;
    public static event Action<string>? LogWarning;
    public static event Action<string>? LogError;

    public static void ConsoleLog(string message)
    {
        Debug.WriteLine(message);
        Log?.Invoke(message);
    }
    private static void ConsoleLogInfo(string message)
    {
        Debug.WriteLine($"Info: {message}");
        LogInfo?.Invoke(message);
    }
    private static void ConsoleLogWarning(string message)
    {
        Debug.WriteLine($"Warning: {message}");
        LogWarning?.Invoke(message);
    }
    private static void ConsoleLogError(string message)
    {
        Debug.WriteLine($"Error: {message}");
        LogError?.Invoke(message);
    }

    public static void SendSimpleMessage(string stringToSend)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (int)ClientToServer.SendMessage);
        message.AddString(stringToSend);

        _client.Send(message); // Sends the message to the server
        // _server.Send(message, <toClientId>); // Sends the message to a specific client
        // _server.SendToAll(message); // Sends the message to all connected clients
        // _server.SendToAll(message, <toClientId>); // Sends the message to all connected clients except the specified one
    }

    public static bool StartServer(ushort port, ushort maxClientCount = 4)
    {
        if(IsServerRunning)
        {
            ConsoleLog("Server already running");
            return false;
        }

        _server.Start(port, maxClientCount);
        return true;
    }

    public static bool StopServer()
    {
        if(!IsServerRunning)
        {
            ConsoleLog("Server is not running");
            return false;
        }

        _server.Stop();
        return true;
    }

    public static bool Connect(string ip)
    {
        if(_client.IsConnected)
        {
            ConsoleLog("Client already connected");
            return false;
        }

        _client.Connect(ip);
        return true;
    }

    public static bool Disconnect()
    {
        if(_client.IsNotConnected)
        {
            ConsoleLog("No connection");
            return false;
        }

        _client.Disconnect();
        return true;
    }
}
