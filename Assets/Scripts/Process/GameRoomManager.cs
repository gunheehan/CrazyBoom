using System;
using System.Collections;
using System.Linq;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameRoomManager : MonoBehaviour
{
    public Action<Player> OnEnteredPlayer = null;
    public Action<string> OnLeftPlayer = null;
    public Action<Player, string, string> OnChangedPlayerState = null;
    public Action<string> OnChangedHost = null;
    public Action<string, string> OnLobbyStateChaged = null;

    private GameLobby gameLobby;
    
    private void Start()
    {
        CheckRoomHost();
        SubscribeToLobbyChanges();
    }

    private void CheckRoomHost()
    {
        string hostId = PlayerSession.Instance.CurrentLobby.HostId;
        string myId = AuthenticationService.Instance.PlayerId;

        lastKnownHostId = hostId;

        Debug.Log("Init Host ID : " + lastKnownHostId);
        Debug.Log("Init My ID : " + myId);
        if(hostId == myId)
            StartCoroutine(HeartbeatLobbyCoroutine(PlayerSession.Instance.CurrentLobby.Id));
    }
    
    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId)
    {
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSeconds(15);
        }
    }
    private LobbyEventCallbacks _callbacks;
    private bool _isSubscribed = false;
    
    private string lastKnownHostId;

    private async void SubscribeToLobbyChanges()
    {
        var callbacks = new LobbyEventCallbacks();

        callbacks.LobbyChanged += lobby =>
        {
            Debug.Log("로비 변경 감지");
            
            if (lobby.HostId.Changed)
            {
                Debug.Log("호스트 변경 감지!");
                Debug.Log("Change Host ID : " + lobby.HostId.Value);
                Debug.Log("current My ID : " + AuthenticationService.Instance.PlayerId);

                if (lobby.HostId.Value == PlayerSession.Instance.PlayerId)
                {
                    Debug.Log("호스트 권한 위임 받음. 하트비트 시작");
                    StartCoroutine(HeartbeatLobbyCoroutine(lobby.HostId.Value));
                }

                lastKnownHostId = lobby.HostId.Value;
            }
            
            if (lobby.PlayerJoined.Added)
            {
                foreach (LobbyPlayerJoined joinedPlayer in lobby.PlayerJoined.Value)
                {
                    Debug.Log($"플레이어 추가됨: {joinedPlayer.Player.Data["nickname"].Value}");
                    OnEnteredPlayer?.Invoke(joinedPlayer.Player);
                }
            }
        };

        await LobbyService.Instance.SubscribeToLobbyEventsAsync(PlayerSession.Instance.CurrentLobby.Id, callbacks);
        
        Lobby updatedLobby = await LobbyService.Instance.GetLobbyAsync(PlayerSession.Instance.CurrentLobby.Id);
        DetectLobbyChanges(PlayerSession.Instance.CurrentLobby, updatedLobby);
    }
    
    private void DetectLobbyChanges(Lobby oldLobby, Lobby newLobby)
    {
        // 1. 플레이어 목록 비교
        var oldPlayerIds = oldLobby.Players.Select(p => p.Id).ToHashSet();
        var newPlayerIds = newLobby.Players.Select(p => p.Id).ToHashSet();

        // 플레이어 추가
        foreach (var added in newPlayerIds.Except(oldPlayerIds))
        {
            Debug.Log($"플레이어 추가됨: {added}");
            OnEnteredPlayer?.Invoke(newLobby.Players.First(p => p.Id == added));
        }

        // 플레이어 제거
        foreach (var removed in oldPlayerIds.Except(newPlayerIds))
        {
            Debug.Log($"플레이어 제거됨: {removed}");
            OnLeftPlayer?.Invoke(removed);
        }

        // 2. 플레이어 상태 변경 감지
        foreach (var player in newLobby.Players)
        {
            var oldPlayer = oldLobby.Players.FirstOrDefault(p => p.Id == player.Id);
            if (oldPlayer != null)
            {
                foreach (var kvp in player.Data)
                {
                    if (oldPlayer.Data.TryGetValue(kvp.Key, out var oldValue))
                    {
                        if (oldValue.Value != kvp.Value.Value)
                        {
                            Debug.Log($"플레이어 상태 변경됨: {player.Id}, {kvp.Key}: {oldValue.Value} → {kvp.Value.Value}");
                            OnChangedPlayerState?.Invoke(player, kvp.Key, kvp.Value.Value);
                        }
                    }
                }
            }
        }

        // 3. 호스트 변경
        if (oldLobby.HostId != newLobby.HostId)
        {
            Debug.Log($"호스트 변경: {oldLobby.HostId} → {newLobby.HostId}");
            OnChangedHost?.Invoke(newLobby.HostId);
        }

        // 4. 로비 메타데이터 변경
        foreach (var kvp in newLobby.Data)
        {
            if (oldLobby.Data.TryGetValue(kvp.Key, out var oldValue))
            {
                if (oldValue.Value != kvp.Value.Value)
                {
                    Debug.Log($"로비 데이터 변경됨: {kvp.Key}: {oldValue.Value} → {kvp.Value.Value}");
                    OnLobbyStateChaged?.Invoke(kvp.Key, kvp.Value.Value);
                }
            }
        }
        
        PlayerSession.Instance.UpdateLobby(newLobby);
    }
    private void DetectLobbyChanges(ILobbyChanges changes)
    {
        // 플레이어가 추가된 경우
        if (changes.PlayerJoined.Changed)
        {
            foreach (LobbyPlayerJoined joinedPlayer in changes.PlayerJoined.Value)
            {
                Debug.Log($"플레이어 추가됨: {joinedPlayer.Player.Profile.Name}");
                OnEnteredPlayer?.Invoke(joinedPlayer.Player);
            }
        }
    }
}
