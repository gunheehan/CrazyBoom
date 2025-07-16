using CrazyBombChat.Models;
using Microsoft.AspNetCore.SignalR;

namespace CrazyBombChat.Hubs;

public class ChatHub : Hub
{
    public async Task JoinLobby(string lobbyId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        await Clients.Group(lobbyId).SendAsync("ReceiveSystem", $"[{Context.ConnectionId}] 입장");
    }

    public async Task SendMessage(ChatMessage message)
    {
        await Clients.Group(message.lobbyId)
            .SendAsync("ReceiveMessage", message.user, message.content);
    }

    public async Task LeaveLobby(string lobbyId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
        await Clients.Group(lobbyId).SendAsync("ReceiveSystem", $"[{Context.ConnectionId}] 퇴장");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}