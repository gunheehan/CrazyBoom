public class LobbyListPresenter : IPresenter
{
    private readonly LobbyListUI _view;
    private readonly LobbyListModel _model;

    public LobbyListPresenter(LobbyListUI view, LobbyListModel model)
    {
        _view = view;
        _model = model;
    }

    public void RefreshLobby() => _model.RefreshLobby();

    public void Subscribe()
    {
        _view.OnClickUpdateList += RefreshLobby;
        _model.OnUpdateLobbyList += _view.UpdateList;
    }

    public void UnSubscribe()
    {
        _view.OnClickUpdateList -= RefreshLobby;
        _model.OnUpdateLobbyList -= _view.UpdateList;
    }
}