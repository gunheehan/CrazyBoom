using UnityEngine;
using UnityEngine.InputSystem;

public class JoyStick : MonoBehaviour
{
    [SerializeField] private InputActionReference inputActionReference;

    // Update is called once per frame
    void Update()
    {
        Vector2 joystickPos = inputActionReference.action.ReadValue<Vector2>();
        Debug.Log(joystickPos);
    }
}
