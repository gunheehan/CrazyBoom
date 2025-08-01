using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : NetworkBehaviour, IPlayerBuff, IPlayer
{
    [SerializeField] private PlayerMove move;
    [SerializeField] private Animator animator;

    private PlayerStat stat;
    private PlayerController controller;
    private AnimationController animation;
    private WalkBehaviour walkBehaviour;

    private string playerid;

    void Awake()
    {
        stat = new PlayerStat();
        controller = new PlayerController();
        walkBehaviour = animator.GetBehaviour<WalkBehaviour>();
        animation = new AnimationController(animator);
        playerid = PlayerSession.Instance.PlayerId;
        move.OnUpdateSpeed(stat.GetPlayerSpeed);
    }

    private void Start()
    {
        InitPlayer(new Vector3(1, 2, 1));
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

    public void OnBuff(BuffItemType buffType)
    {
        stat.OnBuff(buffType);
    }

    public void OnDeBuff(BuffItemType buffType)
    {
        stat.OnDeBuff(buffType);
    }

    private void CreateBomb(InputAction.CallbackContext obj)
    {
        if (stat.GetPlayerBombCount < 1)
            return;

        if (!IsLocalPlayer)
        {
            Debug.Log("Not IsLocalPlayer");
            return;
        }

        HandleCreateBomb();
    }

    private void HandleCreateBomb()
    {
        stat.UseBombStat(true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            PlaneParts plane = hit.collider.gameObject.GetComponent<PlaneParts>();
            plane?.SetBombServerRpc(stat.GetPlayerPower, playerid);
            plane.explodeBomb = OnExplodeBomb;
        }
    }

    private void OnExplodeBomb(string owner)
    {
        if(owner.Equals(playerid))
            stat.UseBombStat(false);
    }

    public void TakeDamage()
    {
        Debug.Log("Player Take Bomb");
    }
}
