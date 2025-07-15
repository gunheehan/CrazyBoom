
    using System;

    public class ChatModel
    {
        public event Action<string, string> RecivedMessage = null;
        
        public void OnRecivedMessage(string sender, string message)
        {
            RecivedMessage?.Invoke(sender, message);
        }
    }