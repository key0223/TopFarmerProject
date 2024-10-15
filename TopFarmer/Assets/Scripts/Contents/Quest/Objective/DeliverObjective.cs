using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverObjective : Objective
{
    public string TargetName { get; private set; } // Npc Name
    public int TargetItemId { get; private set; }
    public DeliverObjective(Quest quest) 
    {
        ItemDeliveryQuest itemDeliveryQuest = (ItemDeliveryQuest)quest;

        TargetName = itemDeliveryQuest.TargetName;
        TargetItemId = itemDeliveryQuest.TargetItemId;
        SuccessToComplete = itemDeliveryQuest.TargetQuantity;
        CurrentSuccess = 0;
        _objectiveAction = new IncrementObjectiveAction();

        Start();
    }
    public override void Start()
    {
        base.Start();
        Managers.Reporter.ItemDeliveredEvent -= OnItemDelivered;
        Managers.Reporter.ItemDeliveredEvent += OnItemDelivered;
    }
    public void OnItemDelivered(string npcName, ItemData item)
    {
        if (!npcName.Contains(TargetName)|| item.itemId != TargetItemId || IsComplete())
            return;

        InventoryItem invenItem = InventoryManager.Instance.FindInventoryItem(Define.InventoryType.INVEN_PLAYER, item.itemId);
        int needValue = SuccessToComplete - CurrentSuccess;
        int amount = Mathf.Min(invenItem._itemQuantity, needValue);
        if (amount < needValue)
            return;

        InventoryManager.Instance.RemoveItem(Define.InventoryType.INVEN_PLAYER,item.itemId, amount);
        ReceiveReport(amount);
        //CurrentSuccess = _objectiveAction.Run(this, CurrentSuccess, amount);

        if(CurrentSuccess >= SuccessToComplete)
        {
            ObjectiveState = Define.ObjectiveState.Complete;
            Managers.Reporter.ItemDeliveredEvent -= OnItemDelivered;
            Managers.Dialogue.MakeDialogueQueue(Owner.ReactionText);
        }

    }
}
