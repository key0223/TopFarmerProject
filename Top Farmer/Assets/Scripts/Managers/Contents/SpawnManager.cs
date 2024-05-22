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
            for (int i = 0; i < 5; i++)
            {
                int randSpawnPosX = Random.Range(Managers.Object.Player.CellPos.x - 10, Managers.Object.Player.CellPos.x + 10);
                int randSpawnPosY = Random.Range(Managers.Object.Player.CellPos.y - 10, Managers.Object.Player.CellPos.y + 10);

                if (Managers.Map.Find(new Vector2Int(randSpawnPosX, randSpawnPosY)) == null)
                {
                    Data.MonsterData monsterData = null;
                    Managers.Data.MonsterDict.TryGetValue(701, out monsterData);


                    GameObject monsterGo = Managers.Resource.Instantiate($"{monsterData.prefabPath}");
                    MaggotController mc = monsterGo.GetComponent<MaggotController>();
                    mc.CellPos = new Vector3Int(randSpawnPosX, randSpawnPosY, 0);
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
