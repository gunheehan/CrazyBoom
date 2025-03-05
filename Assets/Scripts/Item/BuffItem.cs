
using UnityEngine;
using Random = UnityEngine.Random;

public enum BuffItemType
{
    None,
    Speed,
    Power
}
public class BuffItem : MonoBehaviour
{
    private BuffItemType itemType;
    public void SetBuffItem(Vector3 position)
    {
        gameObject.transform.position = position;
        
        int typeIndex = Random.Range(0, (int)BuffItemType.Power);

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