using Unity.Services.Lobbies.Models;

public class PlayerSession
{
    private static PlayerSession instance = null;

    public static PlayerSession Instance
    {
        get
        {
            if (instance == null)
                instance = new PlayerSession();
            return instance;
        }
        private set { }
    }

    public string PlayerId { get; private set; }
    public string PlayerName { get; private set; }
    public Lobby CurrentLobby { get; private set; }

    public void Initialize(string playerId, string playerName)
    {
        PlayerId = playerId;
        PlayerName = playerName;
    }

    public void SetCurrentLobby(Lobby lobby)
    {
        CurrentLobby = lobby;
    }

    public void UpdateLobby(Lobby updatedLobby)
    {
        CurrentLobby = updatedLobby;
    }
}