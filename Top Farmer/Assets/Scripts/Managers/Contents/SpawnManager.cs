using Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SpawnManager
{
    public void SpawnMonsters()
    {
        int randCount = UnityEngine.Random.Range(0, 6);
        int monsterId = 7;
        int randType = UnityEngine.Random.Range(0, 5);

        int randTemplatedId = monsterId * 100 + randType * 10 + 1;

        for (int i = 0; i < randCount; i++)
        {
            int randSpawnPosX = UnityEngine.Random.Range(Managers.Object.Player.CellPos.x - 10, Managers.Object.Player.CellPos.x + 10);
            int randSpawnPosY = UnityEngine.Random.Range(Managers.Object.Player.CellPos.y - 10, Managers.Object.Player.CellPos.y + 10);

            Vector3Int pos = new Vector3Int(randSpawnPosX, randSpawnPosY);

            if (Managers.Map.Find((Vector2Int)pos) == null && Managers.Map.CanGo(pos))
            {
                Data.MonsterData monsterData = null;
                Managers.Data.MonsterDict.TryGetValue(randTemplatedId, out monsterData);

                Monster monster = Monster.MakeMonster(monsterData.monsterId);

                GameObject monsterGo = Managers.Resource.Instantiate($"{monsterData.prefabPath}");
                MonsterController mc = monsterGo.GetComponent<MonsterController>();
                mc.Monster = monster;
                mc.CellPos = new Vector3Int(randSpawnPosX, randSpawnPosY, 0);
                mc.CreatureType = monster.CreatureType;
                mc.SetStat();
                Managers.Object.Add(monsterGo);
            }
        }
    }

    public void SpawnMapObject(string mapName)
    {
        List<PotalData> potals = SpawnPotals(mapName);
        
        foreach(PotalData data in potals)
        {
            GameObject potal = Managers.Resource.Instantiate($"{data.prefabPath}");
            MapChanger mc = Util.GetOrAddComponent<MapChanger>(potal);
            mc.InitPotal(data);
        }

        SpawnItems(mapName);
    }

    List<PotalData> SpawnPotals(string mapName)
    {
        List<PotalData> mapPotal = new List<PotalData>();
        foreach(PotalData potal in Managers.Data.PotalDict.Values)
        {
            if (potal.mapName != mapName)
                continue;
            mapPotal.Add(potal);
        }
        return mapPotal;
    }

    void SpawnItems(string mapName)
    {
        List<ItemData> items = new List<ItemData>();
        List<MapObjectData> mapObjects = Managers.Data.MapObjectDict[mapName];
        if (mapObjects.Count == 0) return;

        foreach(var mapObject in mapObjects)
        {
            ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(mapObject.itemId, out itemData);
            
            ItemType itemType = itemData.itemType;
            if(itemType == ItemType.ITEM_TYPE_TOOL)
            {

            }
            else if(itemType == ItemType.ITEM_TYPE_CROP)
            {

            }
            else if (itemType == ItemType.ITEM_TYPE_SEED)
            {

            }
            else if (itemType == ItemType.ITEM_TYPE_CRAFTING)
            {
                CraftingData data = (CraftingData)itemData;
                GameObject craft = Managers.Resource.Instantiate($"{data.prefabPath}");
                Vector3 pos = new Vector3(mapObject.initPosX, mapObject.initPosY);
                craft.transform.position = pos;

            }
            else if(itemType == ItemType.ITEM_TYPE_FOOD)
            {

            }
            
        }
    }
}
