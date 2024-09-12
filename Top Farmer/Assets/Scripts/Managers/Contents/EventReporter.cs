
using System;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class EventReporter 
{
    public event Action<string, ItemData> ItemDeliveredEvent;
   public void ReportItemPurchased(ItemData item, int quantity)
    {
        QuestType questType = QuestType.Resource;
        object target = item.itemId;
        int successCount = quantity;

        Debug.Log("Item Purchased");
    }

     public void ItemDelivered(string npcName, ItemData item)
    {
        ItemDeliveredEvent?.Invoke(npcName, item);
    }
}
