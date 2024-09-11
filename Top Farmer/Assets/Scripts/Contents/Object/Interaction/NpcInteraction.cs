using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class NpcInteraction : MonoBehaviour,IRaycastable
{

    NPC _npc;

    void Start()
    {
        _npc = GetComponentInParent<NPC>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public CursorType GetCursorType()
    {
        ItemData itemData = InventoryManager.Instance.GetSelectedInventoryItemData(InventoryType.INVEN_PLAYER);
        if (!_npc.ReceivedGift && itemData != null && itemData.canBeDropped)
        {

            return CursorType.Gift;
        }
        else if (!_npc.ReceivedGift)
        {
            return CursorType.Dialogue;
        }
        else { return CursorType.None; }

    }

    public bool HandleRaycast(PlayerController controller)
    {
            Debug.Log("HandleRaycast »£√‚");
        return true;
    }

}
