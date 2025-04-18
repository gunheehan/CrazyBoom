using System;
using Unity.Netcode;
using UnityEngine;


public class PlayerMove : NetworkBehaviour
{
    public event Action<Vector3, float> OnChangeMoveInfo = null;
    private JoyStick joystick;

    private bool isMove = false;
    private Vector3 direction = Vector3.zero;
    private float speed = 3f;

    private void Awake()
    {
        joystick = new JoyStick();
    }

    public void OnUpdateSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void OnEnable()
    {
        joystick.SubscriveEvent();
        joystick.UpdateMove += OnUpdatePlayerDirection;
    }

    private void OnDisable()
    {
        joystick.DeSubscriveEvent();
        joystick.UpdateMove -= OnUpdatePlayerDirection;
    }

    private void OnUpdatePlayerDirection(Vector2 pos)
    {
        if (!IsOwner) return;
        
        if (pos == Vector2.zero)
        {
            direction = Vector3.zero;
            isMove = false;
            OnChangeMoveInfo?.Invoke(direction, 0f);
            return;
        }

        isMove = true;

        float absX = Mathf.Abs(pos.x);
        float absy = Mathf.Abs(pos.y);
        if (absX < absy)
        {
            if (pos.y > 0)
                direction = Vector3.forward;
            else
                direction = Vector3.back;
        }
        else
        {
            if (pos.x > 0)
                direction = Vector3.right;
            else
                direction = Vector3.left;
        }
        
        OnChangeMoveInfo?.Invoke(direction, speed);
    }
}
