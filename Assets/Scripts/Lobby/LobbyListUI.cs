using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListUI : MonoBehaviour
{
    public event Action<string> OnClickJoinLobby = null;
    [SerializeField] private LobbyItem lobbyitemPrefab;
    [SerializeField] private Transform lobbyitemParent;

    private List<LobbyItem> lobbyItemList = new List<LobbyItem>();
    private Stack<LobbyItem> lobbyItemPool = new Stack<LobbyItem>();

    private void ClearLobbyItems()
    {
        foreach (LobbyItem item in lobbyItemList)
        {
            item.gameObject.SetActive(false);
            item.OnClickLobbyItem -= id => OnClickJoinLobby?.Invoke(id);
            lobbyItemPool.Push(item);
        }

        lobbyItemList.Clear();
    }
    
    public void UpdateList(List<Lobby> datalist)
    {
        ClearLobbyItems();

        foreach (Lobby lobby in datalist)
        {
            var lobbyInstance = GetLobbyItem();
            lobbyInstance.Initialise(lobby);
            lobbyInstance.OnClickLobbyItem += id => OnClickJoinLobby?.Invoke(id);
        }
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
