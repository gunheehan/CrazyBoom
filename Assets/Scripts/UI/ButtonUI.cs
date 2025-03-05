using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private Button BombButton;
    void Start()
    {
        BombButton.onClick.AddListener(OnClickBomb);
    }

    private void OnClickBomb()
    {
        InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(Key.Space));
    }
}
