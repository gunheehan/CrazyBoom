using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    public event Action<string> OnClickJoinLobby = null;
    public event Action OnClickUpdateList = null;
    [SerializeField] private LobbyItem lobbyitemPrefab;
    [SerializeField] private Transform lobbyitemParent;
    [SerializeField] private Button listUpdate_button;

    private List<LobbyItem> lobbyItemList = new List<LobbyItem>();
    private Stack<LobbyItem> lobbyItemPool = new Stack<LobbyItem>();

    private void Start()
    {
        listUpdate_button?.onClick.AddListener(OnClickUpdate);
    }
    private void ClearLobbyItems()
    {
        foreach (LobbyItem item in lobbyItemList)
        {
            item.gameObject.SetActive(false);
            item.OnClickLobbyItem -= TryJoinLobby;
            lobbyItemPool.Push(item);
        }

        lobbyItemList.Clear();
    }

    private void OnClickUpdate()
    {
        OnClickUpdateList?.Invoke();
    }
    
    public void UpdateList(List<Lobby> datalist)
    {
        ClearLobbyItems();

        foreach (Lobby lobby in datalist)
        {
            var lobbyInstance = GetLobbyItem();
            lobbyInstance.Initialise(lobby);
            lobbyInstance.OnClickLobbyItem += TryJoinLobby;
        }
    }

    private void TryJoinLobby(string id)
    {
        Debug.Log("Join Target : " + id);
        OnClickJoinLobby?.Invoke(id);
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
