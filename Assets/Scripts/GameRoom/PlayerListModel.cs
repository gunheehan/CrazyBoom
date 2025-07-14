using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerListModel
{
    public event Action<List<Player>> UpdateListUI;
    public event Action<Player> AddListUI;
    public event Action<string, bool> ChangeReadyState;
    
    private List<Player> players;

    public void OnUpdatePlayerList(Lobby lobby)
    {
        players = lobby.Players;
        UpdateListUI?.Invoke(players);
    }

    public void OnAddPlayer(Player player)
    {
        AddListUI?.Invoke(player);
    }
    
    public void OnPlayerStateChanged(Player player, string key, string value)
    {
        if (key == "ready")
        {
            bool isReady = value == "true";
            ChangeReadyState?.Invoke(player.Id, isReady);
        }
    }
}
