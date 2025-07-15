using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    public event Action<string> OnSendMessage = null;
    
    [SerializeField] private InputField chat_inputfield;
    [SerializeField] private Button sendButton;
    [SerializeField] private ChatItem chatitemPrefab;
    [SerializeField] private Transform chatitemParent;
    
    private void Start()
    {
        sendButton.onClick.AddListener(OnClickSendMessage);
    }
    
    private void OnClickSendMessage()
    {
        if (string.IsNullOrEmpty(chat_inputfield.text))
        {
            return;
        }
        
        OnSendMessage?.Invoke(chat_inputfield.text);

        chat_inputfield.text = string.Empty;
    }

    public void OnRecivedMessage(string sender, string message)
    {
        ChatItem newItem = Instantiate(chatitemPrefab, chatitemParent);
        
        newItem.transform.SetAsLastSibling();
        newItem.SetMessage(sender, message);
    }
}
