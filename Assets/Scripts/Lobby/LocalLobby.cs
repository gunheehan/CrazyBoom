using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LocalLobby
{
    private Lobby localLobby;
    public Lobby currentLobby => localLobby;

    public LocalLobby(Lobby currentLobby)
    {
        localLobby = currentLobby;
        SubscribeLobbyEvent();
    }

    public event Action<bool> EnterLobbyEvent = null;
    public event Action<bool> LeftLobbyEvent = null;
    public event Action<List<Lobby>> LobbyListUpdateEvent = null;

    public event Action<Player> EnterPlayerEvent = null;
    public event Action<string> LeftPlayerEvent = null;
    public event Action<Player, string, string> ChangePlayerStateEvent = null;
    public event Action<string> ChnageHostEvent = null;
    public event Action<string, string> LobbyStateChangeEvent = null;

    private async void SubscribeLobbyEvent()
    {
        LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += LobbyChangeEvent;
        
        await LobbyService.Instance.SubscribeToLobbyEventsAsync(localLobby.Id, callbacks);
    }

    private void LobbyChangeEvent(ILobbyChanges lobby)
    {
        if (lobby.HostId.Changed)
        {
            ChnageHostEvent?.Invoke(lobby.HostId.ToString());
        }
            
        if (lobby.PlayerJoined.Added)
        {
            foreach (LobbyPlayerJoined joinedPlayer in lobby.PlayerJoined.Value)
            {
                Debug.Log($"플레이어 추가됨: {joinedPlayer.Player.Data["nickname"].Value}");
                EnterPlayerEvent?.Invoke(joinedPlayer.Player);
            }
        }

        if (lobby.Data.Changed)
        {
            foreach (var lobbychanged in lobby.Data.Value)
            {
                LobbyStateChangeEvent?.Invoke(lobbychanged.Key, lobbychanged.Value.Value.Value);
            }
        }
    }
}
