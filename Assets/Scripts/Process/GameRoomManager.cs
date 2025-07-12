using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class GameRoomManager : MonoBehaviour, IGameRoomEventBroker
{
    public static GameRoomManager Instance { get; private set; }
    
    private Lobby currentLobby;
    private List<Player> currentPlayers;
    private GameEventListener listener;

    public event Action<Player> PlayerJoined;
    public event Action<string> PlayerLeft;
    public event Action<Player, string, string> PlayerStateChanged;
    public event Action<string> HostChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Initialize(PlayerSession.Instance.CurrentLobby);
    }

    private void OnDestroy()
    {
        ResetEvents();
    }

    public async void Initialize(Lobby lobby)
    {
        currentLobby = lobby;
        listener = new GameEventListener();
        listener.OnLobbyChanged += HandleLobbyChanged;

        await listener.StartListening(lobby.Id);
    }

    private void HandleLobbyChanged(ILobbyChanges changes)
    {
        Console.WriteLine("Room Property Changes");

        if (changes.PlayerJoined.Changed)
        {
            foreach (var p in changes.PlayerJoined.Value)
                PlayerJoined?.Invoke(p.Player);
        }

        if (changes.HostId.Changed)
        {
            HostChanged?.Invoke(changes.HostId.ToString());
        }

        if (changes.PlayerData.Changed)
        {
            for (int i = 0; i < changes.PlayerData.Value.Count; i++)
            {
                if(changes.PlayerData.Value[i] == null) continue;

                var newPlayerData = changes.PlayerData.Value[i].ChangedData;
                Debug.Log(currentLobby.Players.Count);
                var player = currentLobby.Players[changes.PlayerData.Value[i].PlayerIndex]; // 동일 인덱스의 플레이어

                if (!newPlayerData.Changed) continue;
                foreach (var kvp in newPlayerData.Value)
                {
                    string key = kvp.Key;
                    string newValue = kvp.Value.Value.Value;

                    Debug.Log(newValue);

                    // 이전 상태가 있다면 비교
                    var oldPlayer = currentPlayers?.Find(p => p.Id == player.Id);
                    if (oldPlayer != null &&
                        oldPlayer.Data.TryGetValue(key, out var oldVal) &&
                        oldVal.Value == newValue)
                    {
                        Debug.Log("변화가 없다");
                        continue; // 변화 없음
                    }

                    // ✅ 변화된 상태 이벤트 발생
                    PlayerStateChanged?.Invoke(player, key, newValue);
                }
            }
        }

        currentPlayers = ClonePlayers(currentLobby.Players);
    }
    
    private List<Player> ClonePlayers(IList<Player> sourcePlayers)
    {
        List<Player> result = new List<Player>(sourcePlayers.Count);
        foreach (var p in sourcePlayers)
        {
            result.Add(new Player(
                id: p.Id,
                data: p.Data?.ToDictionary(
                    d => d.Key,
                    d => new PlayerDataObject(
                        visibility: d.Value.Visibility,
                        value: d.Value.Value
                    )
                ),
                connectionInfo: p.ConnectionInfo
            ));
        }
        return result;
    }
    
    public void ResetEvents()
    {
        PlayerJoined = null;
        PlayerLeft = null;
        PlayerStateChanged = null;
        HostChanged = null;
    }
}