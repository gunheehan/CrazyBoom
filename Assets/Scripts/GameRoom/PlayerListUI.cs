using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerListUI : MonoBehaviour
{
    [SerializeField] private PlayerDataItem item;
    [SerializeField] private Transform contents;

    private Stack<PlayerDataItem> itemStack = new Stack<PlayerDataItem>();
    private Dictionary<string, PlayerDataItem> itemDic = new Dictionary<string, PlayerDataItem>();

    private void ClearList()
    {
        foreach (PlayerDataItem item in itemDic.Values)
        {
            item.Reset();
            itemStack.Push(item);
        }
        itemDic.Clear();
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
            itemDic.Add(player.Id, newItem);
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
        itemDic.Add(player.Id, newItem);
    }

    public void UpdatePlayerState(string playerID, bool isReady)
    {
        if (itemDic.TryGetValue(playerID, out var item))
        {
            item.OnChangeReadyState(isReady);
        }
        else
        {
            Console.WriteLine("Not Found Player. Set Player Ready State");
        }
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
