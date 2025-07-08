using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerListModel
{
    public event Action<List<Player>> UpdateListUI;
    public event Action<Player> AddListUI;
    
    private List<Player> players;

    public void OnUpdatePlayerList(Lobby lobby)
    {
        Debug.Log("PlayerList Model Update");
        players = lobby.Players;
        UpdateListUI?.Invoke(players);
    }

    public void OnAddPlayer(Player player)
    {
        AddListUI?.Invoke(player);
    }
}
