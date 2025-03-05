using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoyStick
{
    public event Action<Vector2> UpdateMove; 
    
    private PlayerController controller;

    public void SubscriveEvent()
    {
        if (controller == null)
            controller = new PlayerController();
        
        controller.Player.Move.started += OnStartMove;
        controller.Player.Move.performed += OnUpdateMove;
        controller.Player.Move.canceled += OnStopMove;
        
        controller.Player.Move.Enable();
    }

    public void DeSubscriveEvent()
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
