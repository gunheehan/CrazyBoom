
public class GameReadyPresenter : IPresenter
{
    private readonly GameReadyUI _view;
    private readonly GameReadyModel _model;

    public GameReadyPresenter(GameReadyUI view, GameReadyModel model)
    {
        _view = view;
        _model = model;
    }
    
    public void Subscribe()
    {
        _view.OnChangeReadyState += _model.GameReadyStateChange;
        _model.UpdateReadyMessage += _view.UpdateReadyText;
    }

    public void UnSubscribe()
    {
        _view.OnChangeReadyState -= _model.GameReadyStateChange;
        _model.UpdateReadyMessage -= _view.UpdateReadyText;
    }
}
    