using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour, IPlayerBuff
{
    [SerializeField] private PlayerMove move;
    [SerializeField] private Animator animator;

    private PlayerStat stat;
    private PlayerController controller;
    private AnimationController animation;
    private WalkBehaviour walkBehaviour;

    void Awake()
    {
        stat = new PlayerStat();
        controller = new PlayerController();
        walkBehaviour = animator.GetBehaviour<WalkBehaviour>();
        animation = new AnimationController(animator);
        move.OnUpdateSpeed(stat.GetPlayerSpeed);
    }

    private void OnEnable()
    {
        stat.OnUpdatePlayerStat += info => move.OnUpdateSpeed(info.speed);

        controller.Player.Attack.performed += CreateBomb;
        controller.Player.Attack.Enable();
        
        move.OnChangeMoveInfo += walkBehaviour.SetMoveInfo;
        move.OnChangeMoveInfo += animation.PlayWalk;
    }

    private void OnDisable()
    {
        stat.OnUpdatePlayerStat -= info => move.OnUpdateSpeed(info.speed); 
        
        controller.Player.Attack.performed -= CreateBomb;
        controller.Player.Attack.Disable();
        
        move.OnChangeMoveInfo -= walkBehaviour.SetMoveInfo;
        move.OnChangeMoveInfo -= animation.PlayWalk;
    }

    public void InitPlayer(Vector3 pos)
    {
        gameObject.transform.position = pos;
        gameObject.SetActive(true);
    }

    private void CreateBomb(InputAction.CallbackContext obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            Debug.Log(hit.collider.gameObject.name);
            PlaneParts plane = hit.collider.gameObject.GetComponent<PlaneParts>();
            plane?.SetBomb(stat.GetPlayerPower);
        }
    }

    public void OnBuff(BuffItemType buffType)
    {
        stat.OnBuff(buffType);
    }

    public void OnDeBuff(BuffItemType buffType)
    {
        stat.OnDeBuff(buffType);
    }
}
