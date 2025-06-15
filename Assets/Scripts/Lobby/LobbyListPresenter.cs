using UnityEngine;

public class LobbyListPresenter : MonoBehaviour
{
    [SerializeField] private LobbyListUI ui;

    private LobbyListModel model = new LobbyListModel();
    
    void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        LobbyManager.Instance.OnEnterdLobby += OnEnteredLobby;
        LobbyManager.Instance.OnLobbyListUpdated += model.UpdateLobbyList;
        ui.OnClickJoinLobby += model.JoinAsync;
        ui.OnClickUpdateList += () => LobbyManager.Instance.RefreshLobbyList();
        model.OnUpdateLobbyList += ui.UpdateList;
    }

    private void OnDisable()
    {
        LobbyManager.Instance.OnEnterdLobby -= OnEnteredLobby;
        LobbyManager.Instance.OnLobbyListUpdated -= model.UpdateLobbyList;
        ui.OnClickJoinLobby -= model.JoinAsync;
        ui.OnClickUpdateList -= () => LobbyManager.Instance.RefreshLobbyList();
        model.OnUpdateLobbyList -= ui.UpdateList;
    }

    private void OnEnteredLobby(bool isEnter)
    {
        ui.gameObject.SetActive(isEnter);
    }
}
