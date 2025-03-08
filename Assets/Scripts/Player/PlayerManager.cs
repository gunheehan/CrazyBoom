using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerMove move;

    private PlayerStat stat;
    private PlayerController controller;

    void Awake()
    {
        stat = new PlayerStat();
        controller = new PlayerController();
        move.OnUpdateSpeed(stat.GetPlayerSpeed);
    }

    private void OnEnable()
    {
        stat.OnUpdatePlayerStat += info => move.OnUpdateSpeed(info.speed);

        controller.Player.Attack.performed += CreateBomb;
        controller.Player.Attack.Enable();
    }

    private void OnDisable()
    {
        stat.OnUpdatePlayerStat -= info => move.OnUpdateSpeed(info.speed); 
        
        controller.Player.Attack.performed -= CreateBomb;
        controller.Player.Attack.Disable();
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
}
