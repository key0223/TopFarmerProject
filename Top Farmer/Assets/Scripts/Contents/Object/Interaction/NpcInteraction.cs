using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class NpcInteraction : MonoBehaviour,IRaycastable
{

    NPC _npc;
    CursorController _cursor;

    void Start()
    {
        _npc = GetComponentInParent<NPC>();
        _cursor = FindObjectOfType<CursorController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public CursorType GetCursorType()
    {
        ItemData itemData = InventoryManager.Instance.GetSelectedInventoryItemData(InventoryType.INVEN_PLAYER);
        
       if(Managers.Quest.ActiveQuests.Count > 0&& itemData !=null)
        {
            foreach(Quest quest in Managers.Quest.ActiveQuests)
            {
                if(quest.Objective.ObjectiveType == ObjectiveType.ItemDelivery && !quest.Objective.IsComplete() )
                {
                    ItemDeliveryQuest deliveryQuest = (ItemDeliveryQuest)quest;

                    if (deliveryQuest.TargetItemId == InventoryManager.Instance.GetSelectedInventoryItemData(InventoryType.INVEN_PLAYER).itemId && _npc.gameObject.name.Contains(deliveryQuest.TargetName))
                    {
                        return CursorType.Quest;
                    }
                }
            }
        }
        else if (!_npc.ReceivedGift && itemData != null && itemData.canBeDropped)
        {

            return CursorType.Gift;
        }
        else if (!_npc.ReceivedGift)
        {
            return CursorType.Dialogue;
        }

        return CursorType.None;

    }

    public bool HandleRaycast(PlayerController controller)
    {
        if (_cursor.CursorType == CursorType.Gift)
        {
            Debug.Log("Gift »£√‚");


        }
        else if(_cursor.CursorType == CursorType.Dialogue)
        {
            int rand = Random.Range(1, 4);
            string name = _npc.gameObject.name.Split('_')[1];
            string dialogueId = $"{name}_Daily_{rand}";

            string dialogue = Managers.Data.StringDict[dialogueId].ko;

            Managers.Dialogue.MakeDialogueQueue(dialogue);
            Managers.Dialogue.DialogueUI.NpcText = name;
        }
        return true;
    }

}
