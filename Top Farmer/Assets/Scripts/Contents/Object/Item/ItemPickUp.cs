using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D coll)
    {
        Item item = coll.GetComponent<Item>();
        if(item != null )
        {
            ItemData itemData = Managers.Data.ItemDict[item.ItemId];

            if(itemData != null && itemData.canBePickedUp)
            {
                InventoryManager.Instance.AddItem(InventoryType.INVEN_PLAYER, item, coll.gameObject);

                SoundManager.Instance.PlaySound(Define.Sound.SOUND_PICKUP);
            }

        }
    }
}
