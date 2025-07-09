using System;
using UnityEngine;
using Unity.Services.Lobbies.Models;

public class GameRoomManager : MonoBehaviour
{
    public static GameRoomManager Instance { get; private set; }

    public event Action<Player> OnPlayerJoined;
    public event Action<string> OnPlayerLeft;
    public event Action<Player, string, string> OnPlayerStateChanged;
    public event Action<string> OnHostChanged;

    private Lobby currentLobby;
    private GameEventListener listener;
    private GameStateTracker stateTracker;

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

    public async void Initialize(Lobby lobby)
    {
        currentLobby = lobby;
        stateTracker = new GameStateTracker(lobby);
        listener = new GameEventListener();

        //listener.OnLobbyChanged += HandleLobbyChanged;
        await listener.StartListening(lobby.Id);
    }

    private void HandleLobbyChanged(LobbyChanges changes)
    {
        var oldLobby = currentLobby;
        currentLobby = stateTracker.Apply(changes);

        // if (changes.PlayerJoined.Changed)
        // {
        //     foreach (var player in changes.PlayerJoined)
        //         OnPlayerJoined?.Invoke(player.Player);
        // }

        if (changes.HostId.Changed)
        {
            OnHostChanged?.Invoke(changes.HostId.ToString());
        }

        foreach (var player in currentLobby.Players)
        {
            var oldPlayer = oldLobby.Players.Find(p => p.Id == player.Id);
            if (oldPlayer != null)
            {
                foreach (var kvp in player.Data)
                {
                    if (oldPlayer.Data.TryGetValue(kvp.Key, out var oldVal))
                    {
                        if (oldVal.Value != kvp.Value.Value)
                        {
                            OnPlayerStateChanged?.Invoke(player, kvp.Key, kvp.Value.Value);
                        }
                    }
                }
            }
        }
    }
}
