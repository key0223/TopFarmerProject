using Data;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager
{
    public void SpawnMonsters()
    {
        int randCount = Random.Range(0, 6);
        int monsterId = 7;
        int randType = Random.Range(0, 5);

        int randTemplatedId = monsterId * 100 + randType * 10 + 1;

        for (int i = 0; i < randCount; i++)
        {
            int randSpawnPosX = Random.Range(Managers.Object.Player.CellPos.x - 10, Managers.Object.Player.CellPos.x + 10);
            int randSpawnPosY = Random.Range(Managers.Object.Player.CellPos.y - 10, Managers.Object.Player.CellPos.y + 10);

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
        List<PotalData> potals = SpawnPotal(mapName);
        
        foreach(PotalData data in potals)
        {
            GameObject potal = Managers.Resource.Instantiate($"{data.prefabPath}");
            MapChanger mc = Util.GetOrAddComponent<MapChanger>(potal);
            mc.InitPotal(data);
        }
       
    }

    List<PotalData> SpawnPotal(string mapName)
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
}
