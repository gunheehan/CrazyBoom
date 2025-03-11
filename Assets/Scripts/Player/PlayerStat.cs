using System;
using UnityEngine;

public class PlayerStatInfo
{
    public int power = 1;
    public float speed = 3;
    public int bombCount = 1;
}
public class PlayerStat : IPlayerBuff
{
    public Action<PlayerStatInfo> OnUpdatePlayerStat = null;
    private PlayerStatInfo stateInfo;

    public int GetPlayerPower
    {
        get
        {
            if (stateInfo == null)
                stateInfo = new PlayerStatInfo();
            return stateInfo.power;
        }
    }

    public float GetPlayerSpeed
    {
        get
        {
            if (stateInfo == null)
                stateInfo = new PlayerStatInfo();
            return stateInfo.speed;
        }
    }

    public int GetPlayerBombCount
    {
        get
        {
            if (stateInfo == null)
                stateInfo = new PlayerStatInfo();
            return stateInfo.bombCount;
        }
    }

    public void UseBombStat(bool isCreated)
    {
        if (isCreated)
            stateInfo.bombCount--;
        else
            stateInfo.bombCount++;
    }
    
    public void OnBuff(BuffItemType buffType)
    {
        if (stateInfo == null)
        {
            Debug.Log("Player Stat is Not Set");
            return;
        }

        switch (buffType)
        {
            case BuffItemType.Speed:
                stateInfo.speed += .5f;
                break;
            case BuffItemType.Power:
                stateInfo.power += 1;
                break;
            case BuffItemType.Bomb:
                stateInfo.bombCount += 1;
                break;
        }
        
        OnUpdatePlayerStat?.Invoke(stateInfo);
    }

    public void OnDeBuff(BuffItemType buffType)
    {
        if (stateInfo == null)
        {
            Debug.Log("Player Stat is Not Set");
            return;
        }
        switch (buffType)
        {
            case BuffItemType.Speed:
                stateInfo.speed = stateInfo.speed > 3 ? stateInfo.speed - 1.5f : stateInfo.speed ;
                break;
            case BuffItemType.Power:
                stateInfo.power = stateInfo.power > 1 ? stateInfo.power - 1 : stateInfo.power ;
                break;
            case BuffItemType.Bomb:
                stateInfo.bombCount = stateInfo.bombCount > 1 ? stateInfo.bombCount - 1 : stateInfo.bombCount ;
                break;
        }
        OnUpdatePlayerStat?.Invoke(stateInfo);
    }
}
