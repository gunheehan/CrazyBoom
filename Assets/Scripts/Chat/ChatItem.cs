using UnityEngine;
using UnityEngine.UI;

public class ChatItem : MonoBehaviour
{
    [SerializeField] private Text sender;
    [SerializeField] private Text message;

    public void SetMessage(string _sender, string _message)
    {
        sender.text = _sender;
        message.text = _message;
        
        gameObject.SetActive(true);
    }
}
