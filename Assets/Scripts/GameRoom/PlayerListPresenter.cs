
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
        //roommanager.OnEnteredPlayer += model.OnAddPlayer;
        _model.UpdateListUI += _view.UpdateUI;
        _model.AddListUI += _view.AddListUI;
    }

    public void UnSubscribe()
    {
        //roommanager.OnEnteredPlayer -= model.OnAddPlayer;
        _model.UpdateListUI -= _view.UpdateUI;
        _model.AddListUI -= _view.AddListUI;
    }
}
