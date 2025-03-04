using UnityEngine;

public interface IPlayerBuff
{
    void OnBuff(PlayerItemType buffType);
    void OnDeBuff(PlayerItemType buffType);
}
