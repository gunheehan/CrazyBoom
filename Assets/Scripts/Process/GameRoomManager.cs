using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameRoomManager : MonoBehaviour, IGameRoomEventBroker
{
    public static GameRoomManager Instance { get; private set; }

    private Lobby currentLobby;
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
        var oldLobby = currentLobby;

        if (changes.PlayerJoined.Changed)
        {
            foreach (var p in changes.PlayerJoined.Value)
                PlayerJoined?.Invoke(p.Player);
        }

        if (changes.HostId.Changed)
        {
            HostChanged?.Invoke(changes.HostId.ToString());
        }

        foreach (var player in currentLobby.Players)
        {
            var oldPlayer = oldLobby.Players.Find(p => p.Id == player.Id);
            if (oldPlayer == null) continue;

            foreach (var kvp in player.Data)
            {
                if (oldPlayer.Data.TryGetValue(kvp.Key, out var oldVal))
                {
                    if (oldVal.Value != kvp.Value.Value)
                    {
                        PlayerStateChanged?.Invoke(player, kvp.Key, kvp.Value.Value);
                    }
                }
            }
        }
    }

    public void ResetEvents()
    {
        PlayerJoined = null;
        PlayerLeft = null;
        PlayerStateChanged = null;
        HostChanged = null;
    }
}