using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerListUI : MonoBehaviour
{
    [SerializeField] private PlayerDataItem item;
    [SerializeField] private Transform contents;

    private Stack<PlayerDataItem> itemStack = new Stack<PlayerDataItem>();
    private List<PlayerDataItem> itemList = new List<PlayerDataItem>();

    private void ClearList()
    {
        foreach (PlayerDataItem item in itemList)
        {
            item.Reset();
            itemStack.Push(item);
        }
        itemList.Clear();
    }
    
    public void UpdateUI(List<Player> players)
    {
        ClearList();

        foreach (Player player in players)
        {
            PlayerDataItem newItem = GetItem();
            
            string nickname = player.Data != null && player.Data.ContainsKey("nickname")
                ? player.Data["nickname"].Value
                : "(이름 없음)";
    
            Debug.Log($"플레이어 ID: {player.Id}, 닉네임: {nickname}");
            newItem.SetPlayerItem(nickname);
        }
    }

    public void AddListUI(Player player)
    {
        PlayerDataItem newItem = GetItem();
            
        string nickname = player.Data != null && player.Data.ContainsKey("nickname")
            ? player.Data["nickname"].Value
            : "(이름 없음)";
    
        Debug.Log($"플레이어 ID: {player.Id}, 닉네임: {nickname}");
        newItem.SetPlayerItem(nickname);
    }

    private PlayerDataItem GetItem()
    {
        PlayerDataItem newItem;
        
        if (itemStack.Count > 0)
            newItem = itemStack.Pop();
        else
            newItem = Instantiate(item, contents);
        
        newItem.transform.SetAsLastSibling();

        return newItem;
    }
}
