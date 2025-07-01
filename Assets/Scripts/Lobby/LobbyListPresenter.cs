using System;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListPresenter : MonoBehaviour, ILobbyUI
{
    [SerializeField] private LobbyListUI ui;

    private LobbyListModel model = new LobbyListModel();

    private void OnConnectedServer(bool isEnter)
    {
        ui.gameObject.SetActive(isEnter);
        model.RefreshLobbyList();
    }

    private void Start()
    {
        LobbyManager.Instance.OnConnectedServer += OnConnectedServer;
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnConnectedServer -= OnConnectedServer;
    }

    public void SubScrive(LocalLobby localLobby)
    {
        localLobby.LobbyListUpdateEvent += model.UpdateLobbyList;
        ui.OnClickJoinLobby += model.JoinAsync;
        // ui.OnClickUpdateList += () => LobbyManager.Instance.RefreshLobbyList();
        model.OnUpdateLobbyList += ui.UpdateList;
    }

    public void DisSubScrive(LocalLobby localLobby)
    {
        localLobby.LobbyListUpdateEvent -= model.UpdateLobbyList;
        ui.OnClickJoinLobby -= model.JoinAsync;
        // ui.OnClickUpdateList -= () => LobbyManager.Instance.RefreshLobbyList();
        model.OnUpdateLobbyList -= ui.UpdateList;
    }
}
