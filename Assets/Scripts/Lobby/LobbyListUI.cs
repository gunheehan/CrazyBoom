using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    public event Action<string, int> OnCreateLobby;
    public event Action<string> OnClickJoinLobby;
    public event Action OnClickUpdateList;

    [SerializeField] private InputField lobbyname_inputfield;
    [SerializeField] private InputField lobbymenber_inputfield;
    [SerializeField] private Button createButton;
    [SerializeField] private Button updateButton;
    [SerializeField] private LobbyItem lobbyitemPrefab;
    [SerializeField] private Transform lobbyitemParent;

    private List<LobbyItem> lobbyItemList = new List<LobbyItem>();
    private Stack<LobbyItem> lobbyItemPool = new Stack<LobbyItem>();
    
    private LobbyListPresenter presenter;

    private void Start()
    {
        createButton.onClick.AddListener(OnClickCreateLobby);
        updateButton.onClick.AddListener(() => OnClickUpdateList?.Invoke());

        var model = new LobbyListModel();
        presenter = new LobbyListPresenter(this, model);
    }
    
    private void OnClickCreateLobby()
    {
        if (lobbyname_inputfield.text == null || lobbymenber_inputfield.text == null)
        {
            Debug.Log("생성 항목을 모두 채워주시오.");
        }
        
        OnCreateLobby?.Invoke(lobbyname_inputfield.text, int.Parse(lobbymenber_inputfield.text));
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
