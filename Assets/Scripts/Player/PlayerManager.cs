using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerMove move;
    [SerializeField] private BombController bomb;

    private PlayerStat stat;
    private PlayerController controller;

    void Awake()
    {
        stat = new PlayerStat();
        controller = new PlayerController();
        move.OnUpdateSpeed(stat.GetPlayerSpeed);
        bomb.OnUpdatePower(stat.GetPlayerPower);
    }

    private void OnEnable()
    {
        stat.OnUpdatePlayerStat += info => move.OnUpdateSpeed(info.speed);
        stat.OnUpdatePlayerStat += info => bomb.OnUpdatePower(info.power);

        controller.Player.Attack.performed += obj => bomb.SetBomb(transform.position);
        controller.Player.Attack.Enable();
    }

    private void OnDisable()
    {
        stat.OnUpdatePlayerStat -= info => move.OnUpdateSpeed(info.speed); 
        stat.OnUpdatePlayerStat -= info => bomb.OnUpdatePower(info.power);
        
        controller.Player.Attack.performed -= obj => bomb.SetBomb(transform.position);
        controller.Player.Attack.Disable();
    }
}
