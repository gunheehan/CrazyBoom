using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class LobbyListModel
{
    public event Action<List<Lobby>> OnUpdateLobbyList;

    public async void RefreshLobby()
    {
        var result = await QueryLobbies();
        OnUpdateLobbyList?.Invoke(result);
    }

    private async Task<List<Lobby>> QueryLobbies()
    {
        var options = new QueryLobbiesOptions
        {
            Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
        };

        var response = await LobbyService.Instance.QueryLobbiesAsync(options);
        return response.Results;
    }
}