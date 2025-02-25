using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoyStick : MonoBehaviour
{
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
    }
    
    private void OnUpdateMove(InputAction.CallbackContext obj)
    {
        Vector2 pos = obj.ReadValue<Vector2>();
    }
    
    private void OnStopMove(InputAction.CallbackContext obj)
    {
        Debug.Log("Stop Move");
    }
}
