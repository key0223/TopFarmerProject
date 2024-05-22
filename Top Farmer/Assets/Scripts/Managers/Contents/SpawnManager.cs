using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager
{
    public void Spawn(bool randomPos)
    {
        int randCount = Random.Range(0, 6);
        int monsterId = 7;
        int randType = Random.Range(0, 5);

        int randTemplatedId = monsterId * 100 + randType * 10 + 1;

        if(randomPos)
        {
            Vector2Int randSpawnPos = new Vector2Int();

            while(true)
            {
                randSpawnPos.x = Random.Range(Managers.Object.Player.CellPos.x - 10, Managers.Object.Player.CellPos.x + 10);
                randSpawnPos.y = Random.Range(Managers.Object.Player.CellPos.y - 10, Managers.Object.Player.CellPos.y + 10);
                
                if(Managers.Map.Find(randSpawnPos) == null)
                {
                    Data.MonsterData monsterData = null;
                    Managers.Data.MonsterDict.TryGetValue(randTemplatedId, out monsterData);
                    MonsterData seedData = (MonsterData)monsterData;

                    GameObject monster = Managers.Resource.Instantiate($"{seedData.prefabPath}");
                    MonsterController mc = monster.GetComponent<MonsterController>();
                }
            }
            //if(Managers.Object.Find(randSpawnPos) == null )
            //{
            //    gameObject.GetComponent<ObjectController>().CellPos = (Vector3Int)randSpawnPos;
            //    Managers.Object.Add(gameObject);

            //}

        }
    }
}
