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

            switch (msg.Type)
            {
                case "join":
                    Username = msg.User;
                    _chatManager.JoinLobby(msg.LobbyId, this);
                    Console.WriteLine($"👤 {Username} joined {msg.LobbyId}");
                    break;

                case "chat":
                    if (!string.IsNullOrEmpty(LobbyId))
                    {
                        string payload = JsonSerializer.Serialize(new ChatMessage
                        {
                            Type = "chat",
                            LobbyId = LobbyId,
                            User = Username!,
                            Content = msg.Content
                        });

                        await _chatManager.BroadcastAsync(LobbyId, payload);
                    }
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