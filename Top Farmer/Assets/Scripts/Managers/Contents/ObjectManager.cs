using Assets.Scripts.Contents.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager 
{
    Dictionary<int, GameObject> _items = new Dictionary<int, GameObject>();
    List<GameObject> _objects = new List<GameObject>();
    public PlayerInfo PlayerInfo { get; set; }
    public PlayerController Player { get; set; }

    public static ItemType GetObjectTypeByItemType(int id)
    {
        int type = (id / 100) & 0x7F;
        return (ItemType)type;
    }

    #region Objects
    public void Add(GameObject go, bool player = false)
    {
        _objects.Add(go);
        if(player)
        {
            Player = go.GetComponent<PlayerController>();
            Player.SetPlayerInfo(PlayerInfo);
        }
    }

    public void Remove(GameObject go)
    {
        _objects.Remove(go);
    }

    public GameObject Find(Vector3Int cellPos)
    {
        //foreach(GameObject obj in _objects)
        //{
        //    CreatureController cc = obj.GetComponent<CreatureController>();
        //    if (cc == null)
        //        continue;

        //    if(cc.CellPos== cellPos)
        //        return obj;
        //}

        foreach(GameObject obj in _objects)
        {
            InteractableObject ic = obj.GetComponent<InteractableObject>();
            if(ic == null) continue;

            ObjectController oc = obj.GetComponent<ObjectController>();
            if (oc.CellPos == cellPos)
                return obj;

        }

        return null;
    }

    public GameObject FindLandObject(Vector3Int cellPos)
    {
        foreach (GameObject obj in _objects)
        {
            PlowedLandController pc = obj.GetComponent<PlowedLandController>();
            if (pc == null)
                continue;

            if (pc.CellPos == cellPos)
                return obj;
        }

        return null;
    }
    


    public void Clear(GameObject go)
    {
        _objects.Clear();
    }

    #endregion

    #region Items

    public void Add(Item item)
    {
        if (_items.ContainsKey(item.Info.itemDbId))
            return;

        ItemType itemType = GetObjectTypeByItemType(item.TemplatedId);

        if(itemType  == ItemType.ITEM_TYPE_TOOL)
        {

        }
        else if(itemType == ItemType.ITEM_TYPE_CROP)
        {
            GameObject go = Managers.Resource.Instantiate($"Object/Land/Crop");
            Crop crop = (Crop)item;

        }
        else if(itemType == ItemType.ITEM_TYPE_SEED)
        {

        }
        else if(itemType ==ItemType.ITEM_TYPE_CRAFTING)
        {

        }
    }
    public GameObject FindObject(Vector3Int cellPos)
    {
        foreach (GameObject obj in _objects)
        {
            ObjectController oc = obj.GetComponent<ObjectController>();
            if (oc == null || obj.gameObject.name == "Land_Plowed")
                continue;

            if (oc.CellPos == cellPos)
                return obj;
        }

        return null;
    }
    #endregion

    public GameObject GetMerchant()
    {
        foreach (GameObject obj in _objects)
        {
            MerchantController mc = obj.GetComponent<MerchantController>();
            if (mc == null)
                continue;

            return obj;
        }

        return null;
    }
}
