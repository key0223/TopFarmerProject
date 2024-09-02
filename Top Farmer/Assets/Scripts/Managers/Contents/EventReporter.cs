
using static Define;

public class EventReporter 
{
   public void ReportItemPurchased(Item item, int quantity)
    {
        QuestType questType = QuestType.Resource;
        object target = item.ItemId;
        int successCount = quantity;

        Managers.Quest.ReceiveReport(questType, target, successCount);
    }
}
