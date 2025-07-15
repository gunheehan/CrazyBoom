
    public class ChatPresenter : IPresenter
    {
        private readonly ChatUI _view;
        private readonly ChatModel _model;
        private ChatServices _services;

        public ChatPresenter(ChatUI view, ChatModel model)
        {
            _view = view;
            _model = model;
        }

        public void SetChatServices(ChatServices services)
        {
            _services = services;
        }
        
        public void Subscribe()
        {
            _services.OnChatReceived += _view.OnRecivedMessage;
            _view.OnSendMessage += _services.SendChat;
        }

        public void UnSubscribe()
        {
            _services.OnChatReceived -= _view.OnRecivedMessage;
            _view.OnSendMessage -= _services.SendChat;
        }
    }