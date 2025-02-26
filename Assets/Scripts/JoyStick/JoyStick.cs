using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoyStick : MonoBehaviour
{
    public event Action<Vector2> UpdateMove; 
    
    private PlayerController controller;

    private void Awake()
    {
        controller = new PlayerController();
    }

    private void OnEnable()
    {
        controller.Player.Move.started += OnStartMove;
        controller.Player.Move.performed += OnUpdateMove;
        controller.Player.Move.canceled += OnStopMove;
        
        controller.Player.Move.Enable();
    }

    private void OnDisable()
    {
        controller.Player.Move.started -= OnStartMove;
        controller.Player.Move.performed -= OnUpdateMove;
        controller.Player.Move.canceled -= OnStopMove;
        
        controller.Player.Move.Disable();
    }

    private void OnStartMove(InputAction.CallbackContext obj)
    {
        Vector2 pos = obj.ReadValue<Vector2>();
        UpdateMove?.Invoke(pos);
    }

    private void OnUpdateMove(InputAction.CallbackContext obj)
    {
        Vector2 pos = obj.ReadValue<Vector2>();
        UpdateMove?.Invoke(pos);
    }

    private void OnStopMove(InputAction.CallbackContext obj)
    {
        UpdateMove?.Invoke(Vector2.zero);
    }
}
