using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager
{
    //private int _playerCoin = 500;
    //public int PlayerCoin
    //{
    //    get { return _playerCoin; }
    //    set
    //    {
    //        _playerCoin = value;
    //    }
    //}
    public Dictionary<int, Item> Items { get; } = new Dictionary<int, Item>();
    public void Add(Item item)
    {
        Item findItem = Find(i => i.TemplatedId == item.TemplatedId && i.Stackable);

        if(findItem != null)
        {
            Items[findItem.ItemDbId].Count += item.Count;
        }
        else
        {
            Items.Add(item.ItemDbId, item);
        }
    }

    public void Remove(Item item)
    {
        Items.Remove(item.ItemDbId);
    }
    public Item Get(int itemDbId)
    {
        Item item = null;
        Items.TryGetValue(itemDbId, out item);

        return item;
    }

    public Item Find(Func<Item, bool> condition)
    {
        foreach (Item item in Items.Values)
        {
            if (condition.Invoke(item))
                return item;
        }
        return null;
    }

    public Item FindDuplicatedItem(int templatedId)
    {
        foreach (Item item in Items.Values)
        {
            if (templatedId == item.TemplatedId && item.Count<item.MaxStack)
                return item;
        }
        return null;
    }
    public int? GetEmptySlot()
    {
        for (int slot = 0; slot < 18; slot++)
        {
            Item item = Items.Values.FirstOrDefault(i => i.Slot == slot);
                if (item == null)
                return slot;

        }

        return null;
    }
    public void Clear()
    {
        Items.Clear();
    }

    public void Update()
    {
        foreach(Item item in Items.Values)
        {
            if(item.Count <=0)
            {
                Items.Remove(item.ItemDbId);
            }
        }
    }

    /*
    public void UpdateRedisItems()
    {
        List<ItemInfo> items = new List<ItemInfo>();
        foreach (Item item in Items.Values)
        {
            items.Add(item.Info);
        }
        UpdateRedisItemsPacketReq packet = new UpdateRedisItemsPacketReq()
        {
            PlayerDbId = Managers.Object.Player.Info.PlayerDbId,
            ItemInfos = items,
        };

        Managers.Web.SendPostRequest<UpdateRedisItemsPacketRes>("item/UpdateRedisItems", packet, (res) =>
        {
            if(res.UpdatedOk)
                Debug.Log("Redis Items Updated");
        });
    }
    */
    public void UpdateInventoryDatabase()
    {
        List<ItemInfo> items = new List<ItemInfo>();
        foreach(Item item in Items.Values)
        {
            items.Add(item.Info);
        }

        UpdateDatabaseItemsReq packet = new UpdateDatabaseItemsReq()
        {
            PlayerDbId = Managers.Object.Player.Info.PlayerDbId,
            ItemInfos = items,
        };

        Managers.Web.SendPostRequest<UpdateDatabaseItemsRes>("item/updateItems", packet, (res) =>
        {
            if (res.UpdatedOk)
                Debug.Log("Database Items Updated");
        });
    }
}
