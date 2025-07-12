using UnityEngine;
using UnityEngine.UI;

public class PlayerDataItem : MonoBehaviour
{
    [SerializeField] private Text playerName_text;
    [SerializeField] private Toggle readyStateToggle;
    
    public void Reset()
    {
        playerName_text.text = string.Empty;
        gameObject.SetActive(false);
    }

    public void SetPlayerItem(string name)
    {
        playerName_text.text = name;
        gameObject.SetActive(true);
    }

    public void OnChangeReadyState(bool isReady)
    {
        Debug.Log("Player Ready State Change : " + isReady);
        readyStateToggle.isOn = isReady;
    }
}
