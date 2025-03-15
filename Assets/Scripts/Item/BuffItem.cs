
using UnityEngine;
using Random = UnityEngine.Random;

public enum BuffItemType
{
    Speed,
    Power,
    Bomb,
    None
}
public class BuffItem : MonoBehaviour, IBuff
{
    [SerializeField] private BuffItemType itemType;
    public void SetBuffItem()
    {
        int typeIndex = Random.Range(0, 8);
        if(typeIndex >= (int)BuffItemType.None)
            return;
        
        itemType = (BuffItemType)typeIndex;
        
        gameObject.SetActive(true);
    }
    
    public void TakeDamege()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IPlayerBuff playerBuff = other.gameObject.GetComponent<IPlayerBuff>();
            if(playerBuff != null)
                playerBuff.OnBuff(itemType);
            
            gameObject.SetActive(false);
        }
    }
}