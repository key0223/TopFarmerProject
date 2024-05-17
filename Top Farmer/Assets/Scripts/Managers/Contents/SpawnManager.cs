using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager
{
    
    Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();



    public void Spawn(GameObject gameObject, bool randomPos)
    {
        if (gameObject == null) return;

        if(randomPos)
        {
            Vector2Int randSpawnPos = new Vector2Int();

            randSpawnPos.x = Random.Range(Managers.Object.Player.CellPos.x - 10, Managers.Object.Player.CellPos.x + 10);
            randSpawnPos.y = Random.Range(Managers.Object.Player.CellPos.y - 10, Managers.Object.Player.CellPos.y + 10);

            if(Managers.Object.Find(randSpawnPos) == null )
            {
                gameObject.GetComponent<ObjectController>().CellPos = (Vector3Int)randSpawnPos;
                Managers.Object.Add(gameObject);

            }

        }
    }

   
}
