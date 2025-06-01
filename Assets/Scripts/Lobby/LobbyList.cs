using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private LobbyItem lobbyitemPrefab;
    [SerializeField] private Transform lobbyitemParent;

    private List<LobbyItem> lobbyItemList = new List<LobbyItem>();
    private Stack<LobbyItem> lobbyItemPool = new Stack<LobbyItem>();
    
    private bool isRefreshing = false;
    private bool isJoin = false;

    private void OnEnable()
    {
        LobbyManager.Instance.OnLobbyListUpdated += UpdateLobbyList;
    }

    private void OnDisable()
    {
        LobbyManager.Instance.OnLobbyListUpdated -= UpdateLobbyList;
    }

    public async void RefreshLobby()
    {
        if (isRefreshing)
            return;

        isRefreshing = true;
        ClearLobbyItems();
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

            foreach (Transform child in lobbyitemParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                var lobbyInstance = GetLobbyItem();
                lobbyInstance.Initialise(this, lobby);
            }
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
        ClearLobbyItems();

        foreach (Lobby lobby in lobbies)
        {
            var lobbyInstance = GetLobbyItem();
            lobbyInstance.Initialise(this, lobby);
        }
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoin)
            return;

        isJoin = true;
        try
        {
            var joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            isJoin = false;
            throw;
        }
    }

    private void ClearLobbyItems()
    {
        foreach (LobbyItem item in lobbyItemList)
        {
            item.gameObject.SetActive(false);
            lobbyItemPool.Push(item);
        }

        lobbyItemList.Clear();
    }

    private LobbyItem GetLobbyItem()
    {
        LobbyItem item;

        if (lobbyItemPool.Count > 0)
            item = lobbyItemPool.Pop();
        else
            item = Instantiate(lobbyitemPrefab, lobbyitemParent);
        
        item.transform.SetAsLastSibling();
        
        lobbyItemList.Add(item);
        return item;
    }
}
