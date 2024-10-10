using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class MonsterGenerator : MonoBehaviour
{
    public int monsterId;


    public void GenerateMonster()
    {

        if (monsterId == 0)
            return;

        GameObject transformRoot = GameObject.FindGameObjectWithTag("MonstersParentTransform");

        switch (monsterId)
        {
            case 801:
                {
                    GameObject monster = Managers.Resource.Instantiate("Creature/Monster/Slime", transformRoot.transform);
                    monster.transform.position = SpwanPosition();
                    MonsterStat monsterStat = monster.GetComponentInChildren<MonsterStat>();
                    monsterStat.SetStat(monsterId);
                }
                break;
            case 802:
                {
                    GameObject monster = Managers.Resource.Instantiate("Creature/Monster/Bug", transformRoot.transform);
                    monster.transform.position = SpwanPosition();
                    MonsterStat monsterStat = monster.GetComponentInChildren<MonsterStat>();
                    monsterStat.SetStat(monsterId);
                }
                break;
            case 803:
                {
                    GameObject monster = Managers.Resource.Instantiate("Creature/Monster/Ghost", transformRoot.transform);
                    monster.transform.position = SpwanPosition();
                    MonsterStat monsterStat = monster.GetComponentInChildren<MonsterStat>();
                    monsterStat.SetStat(monsterId);
                }
                break;
        }
    }

    Vector3 SpwanPosition ()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");

        Vector3 spwanPosition = new Vector3(target.transform.position.x + Random.Range(-5f, 5f),
                                            target.transform.position.y + Random.Range(-5f, 5f), 0f);

        return spwanPosition;

    }
}
