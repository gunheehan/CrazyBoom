using CrazyBombChat.Connection;

namespace CrazyBombChat.Services;

public class ChatManager
{
    private readonly Dictionary<string, List<ClientConnection>> _lobbies = new();

    public void JoinLobby(string lobbyId, ClientConnection client)
    {
        if (!_lobbies.ContainsKey(lobbyId))
            _lobbies[lobbyId] = new List<ClientConnection>();

        _lobbies[lobbyId].Add(client);
        client.LobbyId = lobbyId;
    }

    public void LeaveLobby(string lobbyId, ClientConnection client)
    {
        if (_lobbies.TryGetValue(lobbyId, out var clients))
        {
            clients.Remove(client);
            if (clients.Count == 0)
                _lobbies.Remove(lobbyId);
        }
    }

    public async Task BroadcastAsync(string lobbyId, string message)
    {
        if (!_lobbies.TryGetValue(lobbyId, out var clients)) return;

        foreach (var client in clients)
        {
            await client.SendAsync(message);
        }
    }
}