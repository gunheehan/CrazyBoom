
public class PlayerListPresenter : IPresenter
{
    private readonly PlayerListUI _view;
    private readonly PlayerListModel _model;
    
    public PlayerListPresenter(PlayerListUI view, PlayerListModel model)
    {
        _view = view;
        _model = model;
    }
    public void Subscribe()
    {
        GameRoomManager.Instance.PlayerJoined += _model.OnAddPlayer;
        _model.UpdateListUI += _view.UpdateUI;
        _model.AddListUI += _view.AddListUI;

        Initialize();
    }

    public void UnSubscribe()
    {
        GameRoomManager.Instance.PlayerJoined -= _model.OnAddPlayer;
        _model.UpdateListUI -= _view.UpdateUI;
        _model.AddListUI -= _view.AddListUI;
    }

    private void Initialize()
    {
        _model.OnUpdatePlayerList(PlayerSession.Instance.CurrentLobby);
    }
}
