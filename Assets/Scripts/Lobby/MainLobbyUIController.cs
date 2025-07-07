using System.Collections.Generic;
using UnityEngine;

public class MainLobbyUIController : MonoBehaviour
{
    [SerializeField] private LobbyListUI lobbyUI;

    private List<IPresenter> presenters;

    private void Start()
    {
        presenters = new List<IPresenter>();
        
        var lobbyListModel = new LobbyListModel();
        LobbyListPresenter lobbyListPresenter = new LobbyListPresenter(lobbyUI, lobbyListModel);
        presenters.Add(lobbyListPresenter);

        var lobbyconnectModel = new LobbyConnectModel();
        LobbyConnectPresenter lobbyConnectPresenter = new LobbyConnectPresenter(lobbyUI, lobbyconnectModel);
        presenters.Add(lobbyConnectPresenter);

        foreach (var presenter in presenters)
        {
            presenter.Subscribe();
        }
    }

    private void OnDestroy()
    {
        foreach (var presenter in presenters)
        {
            presenter.UnSubscribe();
        }
    }
}
