using UnityEngine;

public class LobbyListPresenter : MonoBehaviour, ILobbyUI
{
    [SerializeField] private LobbyListUI ui;

    private LobbyListModel model = new LobbyListModel();

    public void SubScrive(LocalLobby localLobby)
    {
        localLobby.LobbyListUpdateEvent += model.UpdateLobbyList;
        ui.OnClickJoinLobby += model.JoinAsync;
        ui.OnClickUpdateList += model.RefreshLobby;
        model.OnUpdateLobbyList += ui.UpdateList;
    }

    public void DisSubScrive(LocalLobby localLobby)
    {
        localLobby.LobbyListUpdateEvent -= model.UpdateLobbyList;
        ui.OnClickJoinLobby -= model.JoinAsync;
        ui.OnClickUpdateList -= model.RefreshLobby;
        model.OnUpdateLobbyList -= ui.UpdateList;
    }
}
