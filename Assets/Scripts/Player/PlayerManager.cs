using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerMove move;
    [SerializeField] private BombController bomb;

    private PlayerStat stat;

    void Awake()
    {
        stat = new PlayerStat();
        move.OnUpdateSpeed(stat.GetPlayerSpeed);
        bomb.OnUpdatePower(stat.GetPlayerPower);
    }

    private void OnEnable()
    {
        stat.OnUpdatePlayerStat += info => move.OnUpdateSpeed(info.speed);
        stat.OnUpdatePlayerStat += info => bomb.OnUpdatePower(info.power);
    }

    private void OnDisable()
    {
        stat.OnUpdatePlayerStat -= info => move.OnUpdateSpeed(info.speed); 
        stat.OnUpdatePlayerStat -= info => bomb.OnUpdatePower(info.power);
    }
}
