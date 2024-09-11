using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverObjective : Objective
{
    public string TargetName { get; private set; } // Npc Name
    public int TargetItemId { get; private set; }
    public DeliverObjective(Quest quest) : base(quest)
    {
        ItemDeliveryQuest itemDeliveryQuest = (ItemDeliveryQuest)Owner;

        TargetName = itemDeliveryQuest.TargetName;
        TargetItemId = itemDeliveryQuest.TargetItemId;
        SuccessToComplete = itemDeliveryQuest.TargetQuantity;
        CurrentSuccess = 0;
        _objectiveAction = new IncrementObjectiveAction();
    }
    public override void Start()
    {
        base.Start();
        Managers.Reporter.ItemDeliveredEvent += OnItemDelivered;
    }
    public void OnItemDelivered(string npcName, InventoryItem item)
    {
        if (npcName != TargetName || item._itemId != TargetItemId)
            return;

        int needValue = SuccessToComplete - CurrentSuccess;
        int amount = Mathf.Min(item._itemQuantity, needValue);
        if (amount < needValue)
            return;

        item._itemQuantity -= amount;
        CurrentSuccess = _objectiveAction.Run(this, CurrentSuccess, amount);

    }
}
