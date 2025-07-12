using System;
using UnityEngine;
using UnityEngine.UI;

public class GameReadyUI : MonoBehaviour
{
    public event Action<bool> OnChangeReadyState = null;
    
    [SerializeField] private Text readyStateText;
    [SerializeField] private Toggle readyToggle;
    
    void Start()
    {
        readyToggle.onValueChanged.AddListener((ison)=>OnChangeToggle(ison));
    }

    public void UpdateReadyText(string text)
    {
        readyStateText.text = text;
    }

    private void OnChangeToggle(bool ison)
    {
        OnChangeReadyState?.Invoke(ison);
    }
}
