using UnityEngine;

public interface IPlayerBuff
{
    void OnBuff(BuffItemType buffType);
    void OnDeBuff(BuffItemType buffType);
}
