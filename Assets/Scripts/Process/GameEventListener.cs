using System;
using System.Threading.Tasks;
using Unity.Services.Lobbies;

public class GameEventListener
{
    public event Action<ILobbyChanges> OnLobbyChanged;
    private LobbyEventCallbacks callbacks;

    public async Task StartListening(string lobbyId)
    {
        callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += changes => OnLobbyChanged?.Invoke(changes);

        await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyId, callbacks);
    }

    public async Task StopListening(string lobbyId)
    {
        await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
    }
}