using Unity.Services.Lobbies.Models;

public class PlayerSession
{
    public static PlayerSession Instance { get; private set; }

    public string PlayerId { get; private set; }
    public string PlayerName { get; private set; }
    public Lobby CurrentLobby { get; private set; }

    public void Initialize(string playerId, string playerName, Lobby lobby)
    {
        PlayerId = playerId;
        PlayerName = playerName;
        CurrentLobby = lobby;
    }

    public void UpdateLobby(Lobby updatedLobby)
    {
        CurrentLobby = updatedLobby;
    }
}