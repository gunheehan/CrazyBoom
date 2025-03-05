using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private JoyStick joystick;

    private bool isMove = false;
    private Vector3 direction = Vector3.zero;
    private float speed = 3f;

    public void OnUpdateSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void OnEnable()
    {
        joystick.UpdateMove += OnUpdatePlayerDirection;
    }

    private void OnDisable()
    {
        joystick.UpdateMove -= OnUpdatePlayerDirection;
    }

    private void FixedUpdate()
    {
        if (isMove)
        {
            gameObject.transform.position += direction * (speed * Time.deltaTime);
        }
    }

    private void OnUpdatePlayerDirection(Vector2 pos)
    {
        if (pos == Vector2.zero)
        {
            direction = Vector3.zero;
            isMove = false;
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
    }
}
