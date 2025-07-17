using System.Net.WebSockets;
using System.Text;
using CrazyBombChat.Models;
using CrazyBombChat.Services;
using System.Text.Json;

namespace CrazyBombChat.Connection;

public class ClientConnection
{
    private readonly WebSocket _socket;
    private readonly ChatManager _chatManager;

    public string? LobbyId { get; set; }
    public string? Username { get; set; }

    public ClientConnection(WebSocket socket, ChatManager manager)
    {
        _socket = socket;
        _chatManager = manager;
    }

    public async Task ProcessAsync()
    {
        var buffer = new byte[1024 * 4];

        while (_socket.State == WebSocketState.Open)
        {
            var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
                break;

            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var msg = JsonSerializer.Deserialize<ChatMessage>(json);

            if (msg == null) continue;

            switch (msg.type)
            {
                case "join":
                    Username = msg.user;
                    LobbyId = msg.lobbyId;
                    _chatManager.JoinLobby(msg.lobbyId, this);
                    Console.WriteLine($"👤 {Username} joined {msg.lobbyId}");
                    break;

                case "chat":
                    if (!string.IsNullOrEmpty(LobbyId))
                    {
                        string payload = JsonSerializer.Serialize(new ChatMessage
                        {
                            type = "chat",
                            lobbyId = LobbyId,
                            user = Username!,
                            content = msg.content
                        });
                        Console.WriteLine("SendMessage" + payload);
                        await _chatManager.BroadcastAsync(LobbyId, payload);
                    }

                    break;
                case "leave":
                    Username = msg.user;
                    _chatManager.LeaveLobby(msg.lobbyId, this);
                    Console.WriteLine($"👤 {Username} leave {msg.lobbyId}");
                    break;
            }
        }

        if (!string.IsNullOrEmpty(LobbyId))
            _chatManager.LeaveLobby(LobbyId, this);
    }

    public async Task SendAsync(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        await _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}