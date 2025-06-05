using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListModel
{
    public Action<List<Lobby>> OnUpdateLobbyList = null;
    
    private bool isRefreshing = false;

    public async void RefreshLobby()
    {
        if (isRefreshing)
            return;

        isRefreshing = true;
        try
        {
            var options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0")
            };

            var lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            OnUpdateLobbyList?.Invoke(lobbies.Results);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            isRefreshing = false;
            throw;
        }
        
        isRefreshing = false;
    }
    
    public void UpdateLobbyList(List<Lobby> lobbies)
    {
        OnUpdateLobbyList?.Invoke(lobbies);
    }
}
