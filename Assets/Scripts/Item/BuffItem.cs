
using UnityEngine;
using Random = UnityEngine.Random;

public enum BuffItemType
{
    Speed,
    Power,
    None
}
public class BuffItem : MonoBehaviour
{
    [SerializeField] private BuffItemType itemType;
    public void SetBuffItem()
    {
        // int typeIndex = Random.Range(0, 10); // 0부터 9까지의 값 생성
        // if(typeIndex >= (int)BuffItemType.None)
        //     return;
        
        int typeIndex = Random.Range(0, 1); // 0부터 9까지의 값 생성
        itemType = (BuffItemType)typeIndex;
        
        gameObject.SetActive(true);
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