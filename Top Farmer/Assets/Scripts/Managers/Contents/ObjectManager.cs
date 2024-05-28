using Assets.Scripts.Contents.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class ObjectManager 
{
    //Dictionary<int, GameObject> _items = new Dictionary<int, GameObject>();

    public PlayerInfo PlayerInfo { get; set; }
    public PlayerController Player { get; set; }

    List<GameObject> _objects = new List<GameObject>();

    //Dictionary<int, ObjectController> _objects = new Dictionary<int, ObjectController>();
    Dictionary<int, CreatureController> _creatures = new Dictionary<int, CreatureController>();
    Dictionary<int, MonsterController> _monsters = new Dictionary<int, MonsterController>();

    int _counter = 0; //
    int GenerateId(ObjectType type)
    {
            return ((int)type << 24) | (_counter++);
    }
    public static ItemType GetObjectTypeByItemType(int id)
    {
        int type = (id / 100) & 0x7F;
        return (ItemType)type;
    }
   

    public void Add(GameObject go, bool player = false)
    {
        ObjectController oc = go.GetComponent<ObjectController>();
        oc.ObjectId = GenerateId(oc.ObjectType);
        Debug.Log(oc.ObjectId);

        if(oc.ObjectType == ObjectType.OBJECT_TYPE_PLAYER && player)
        {
            Player = go.GetComponent<PlayerController>();
            Player.SetPlayerInfo(PlayerInfo);

            CreatureController cc = (CreatureController)Player;
            _creatures.Add(cc.ObjectId, cc);
        }
        else if(oc.ObjectType == ObjectType.OBJECT_TYPE_OBJECT)
        {
            _objects.Add(go);
        }
        else if(oc.ObjectType == ObjectType.OBJECT_TYPE_ITEM)
        {
            _objects.Add(go);
        }
        else if(oc.ObjectType == ObjectType.OBJECT_TYPE_CREATURE )
        {
            CreatureController cc = (CreatureController)oc;

            switch (cc.CreatureType)
            {
                case CreatureType.CREATURE_TYPE_NPC:
                    _creatures.Add(cc.ObjectId, cc);
                    break;
                case CreatureType.CREATURE_TYPE_MONSTER:
                    {
                        MonsterController mc = (MonsterController)oc;
                        _monsters.Add(mc.ObjectId, mc);
                    }
                    break;
            }
        }

        if(oc.ObjectType != ObjectType.OBJECT_TYPE_ITEM)
        {
            Managers.Map.Add(go);
        }
    }
    public GameObject Find(Vector3Int cellPos)
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
    public GameObject FindCreature(Vector3Int cellPos)
    {
        foreach(CreatureController obj in _creatures.Values)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if(cc.CellPos == cellPos)
                return obj.gameObject;
        }
        return null;
    }

    public GameObject FindMonster(Vector3Int cellPos)
    {
        foreach (MonsterController obj in _monsters.Values)
        {
            MonsterController mc = obj.GetComponent<MonsterController>();
            if (mc == null)
                continue;

            if (mc.CellPos == cellPos)
                return obj.gameObject;
        }
        return null;
    }
    public GameObject FindLand(Vector3Int cellPos)
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

    // GameObject를 인자로 받고 그에 대한 결과값을 bool로 반환
    public GameObject Find(Func<GameObject, bool> condition) 
    {
        foreach(GameObject obj in _objects)
        {
            if(condition.Invoke(obj))
                return obj;
        }
        return null;
    }

    public GameObject FindCreature(Func<GameObject, bool> condition)
    {
        foreach(CreatureController obj in _creatures.Values)
        {
            if (condition.Invoke(obj.gameObject))
                return obj.gameObject;
        }
        return null;
    }

    public void Remove(GameObject go)
    {
        _objects.Remove(go);
    }
    public void RemoveMonster(GameObject go)
    {
        MonsterController mc = go.GetComponent<MonsterController>();

        _monsters.Remove(mc.ObjectId);
        Managers.Map.Remove(go);
    }

    //public GameObject Find(Vector3Int cellPos)
    //{
    //    foreach(GameObject obj in _objects)
    //    {
    //        InteractableObject ic = obj.GetComponent<InteractableObject>();
    //        if(ic == null) continue;

    //        ObjectController oc = obj.GetComponent<ObjectController>();
    //        if (oc.CellPos == cellPos)
    //            return obj;

    //    }

    //    return null;
    //}

    public void Clear(GameObject go)
    {
        _objects.Clear();
    }

    //public GameObject GetMerchant()
    //{
    //    foreach (GameObject obj in _objects)
    //    {
    //        MerchantController mc = obj.GetComponent<MerchantController>();
    //        if (mc == null)
    //            continue;

    //        return obj;
    //    }

    //    return null;
    //}
}
