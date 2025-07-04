public class LobbyListPresenter
{
    private readonly LobbyListUI _view;
    private readonly LobbyListModel _model;

    public LobbyListPresenter(LobbyListUI view, LobbyListModel model)
    {
        _view = view;
        _model = model;

        // View 이벤트 구독
        _view.OnClickUpdateList += RefreshLobby;
        _view.OnClickJoinLobby += JoinLobby;

        // Model 이벤트 구독
        _model.OnUpdateLobbyList += _view.UpdateList;
    }

    public void RefreshLobby() => _model.RefreshLobby();
    public void JoinLobby(string id) => _model.JoinAsync(id);

    public void Dispose()
    {
        _view.OnClickUpdateList -= RefreshLobby;
        _view.OnClickJoinLobby -= JoinLobby;
        _model.OnUpdateLobbyList -= _view.UpdateList;
    }
}