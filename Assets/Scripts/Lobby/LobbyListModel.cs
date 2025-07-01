using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListModel
{
    public Action<List<Lobby>> OnUpdateLobbyList = null;
    
    private bool isRefreshing = false;
    private bool isJoin = false;

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
    
    public async void JoinAsync(string lobbyid)
    {
        Debug.Log("Join State : " + isJoin);
        if (isJoin)
            return;

        isJoin = true;
        try
        {
            LobbyManager.Instance.JoinLobbyByCode(lobbyid);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            isJoin = false;
            throw;
        }

        isJoin = false;
    }
    
    public async Task RefreshLobbyList()
    {
        Debug.Log("로비 리스트 업데이트");
        List<Lobby> lobbies = await QueryLobbies();
        OnUpdateLobbyList?.Invoke(lobbies); // UI 업데이트를 위해 이벤트 호출
    }
    
    // 4️⃣ 공개된 로비 목록 검색
    public async Task<List<Lobby>> QueryLobbies()
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
