using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataItem : MonoBehaviour
{
    [SerializeField] private Text playerName_text;

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
}
