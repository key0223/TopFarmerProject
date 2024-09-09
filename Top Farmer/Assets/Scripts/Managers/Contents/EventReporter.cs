
using UnityEngine;
using static Define;

public class EventReporter 
{
   public void ReportItemPurchased(ItemData item, int quantity)
    {
        QuestType questType = QuestType.Resource;
        object target = item.itemId;
        int successCount = quantity;

        Debug.Log("Item Purchased");
    }
}
