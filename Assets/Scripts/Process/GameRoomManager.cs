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
                LobbyPlayerChanges outData = null;
                changes.PlayerData.Value.TryGetValue(i, out outData);
                if(outData == null)
                    continue;

                var newPlayerData = outData.ChangedData;
                Debug.Log(currentLobby.Players.Count);
                var player = currentLobby.Players[changes.PlayerData.Value[i].PlayerIndex];

                if (!newPlayerData.Changed) continue;
                foreach (var kvp in newPlayerData.Value)
                {
                    string key = kvp.Key;
                    string newValue = kvp.Value.Value.Value;

                    Debug.Log(newValue);

                    var oldPlayer = currentPlayers?.Find(p => p.Id == player.Id);
                    if (oldPlayer != null &&
                        oldPlayer.Data.TryGetValue(key, out var oldVal) &&
                        oldVal.Value == newValue)
                    {
                        Debug.Log("변화가 없다");
                        continue;
                    }

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