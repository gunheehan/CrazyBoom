using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    
    public async Task RefreshLobbyList()
    {
        Debug.Log("로비 리스트 업데이트");
        List<Lobby> lobbies = await QueryLobbies();
        LobbyListUpdateEvent?.Invoke(lobbies); // UI 업데이트를 위해 이벤트 호출
    }
    
    private async Task<List<Lobby>> QueryLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);

            return response.Results;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("로비 검색 실패: " + e.Message);
            return new List<Lobby>();
        }
    }
}
