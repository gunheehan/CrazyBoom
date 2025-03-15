using UnityEngine;

public class WalkBehaviour : StateMachineBehaviour
{
    private GameObject avatar;

    //속도 배율
    [SerializeField] private float speedMagnification = 1f;
    private Vector3 direction = Vector3.zero;
    private float moveSpeed = 0f;
    
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        avatar = animator.gameObject;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UpdateMoveState();
    }
    
    public void SetMoveInfo(Vector3 direct, float speed)
    {
        direction = direct;
        moveSpeed = speed;
    }
    
    private void UpdateMoveState()
    {
        if (direction.sqrMagnitude <= 0)
            return;

        Quaternion rotation = Quaternion.LookRotation(direction);
        avatar.transform.rotation = rotation;
        avatar.transform.position = Vector3.Lerp(avatar.transform.position, avatar.transform.position + direction, (moveSpeed*speedMagnification )* Time.deltaTime);
    }
}
