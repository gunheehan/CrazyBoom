
public class LobbyConnectPresenter : IPresenter
{
    private readonly LobbyListUI _view;
    private readonly LobbyConnectModel _model;
    
    public LobbyConnectPresenter(LobbyListUI view, LobbyConnectModel model)
    {
        _view = view;
        _model = model;
    }
    private void CreateLobby(string lobbyName, int maxPlayer) => _model.CreateLobby(lobbyName, maxPlayer);
    private void JoinLobby(string id) => _model.JoinLobbyByCode(id);

    public void Subscribe()
    {
        _view.OnCreateLobby += CreateLobby;
        _view.OnClickJoinLobby += JoinLobby;
    }

    public void UnSubscribe()
    {
        _view.OnCreateLobby -= CreateLobby;
        _view.OnClickJoinLobby -= JoinLobby;
    }
}
