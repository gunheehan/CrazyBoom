
using UnityEngine;
using Random = UnityEngine.Random;

public enum BuffItemType
{
    Speed,
    Power,
    Bomb,
    None
}
public class BuffItem : MonoBehaviour
{
    [SerializeField] private BuffItemType itemType;
    public void SetBuffItem()
    {
        int typeIndex = Random.Range(0, 10); // 0부터 9까지의 값 생성
        if(typeIndex >= (int)BuffItemType.None)
            return;
        
        itemType = (BuffItemType)typeIndex;
        
        gameObject.SetActive(true);
    }

    public void OnDamaged()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IPlayerBuff playerBuff = other.gameObject.GetComponent<IPlayerBuff>();
            if(playerBuff != null)
                playerBuff.OnBuff(itemType);
            
            gameObject.SetActive(false);
        }
    }
}