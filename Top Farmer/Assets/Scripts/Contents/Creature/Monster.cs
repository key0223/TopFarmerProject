using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster 
{
    public int TemplatedId { get; protected set; }
    public string Name { get; private set; }
    public ObjectType ObjectType { get; private set; }
    public CreatureType CreatureType { get; private set; }
    public MonsterType MonsterType { get; private set; }
    public int Level { get; private set; }  
    public int MaxHp { get; private set; }
    public int Attack { get; private set; } 
    public  float Speed { get; private set; }
    public int TotalExp { get; private set; }
   
    public Monster(MonsterType monsterType)
    {
        MonsterType = monsterType;
    }
    public Monster MakeMonster(int templatedId)
    {
        Monster monster = null;
        MonsterData monsterData = null;
        Managers.Data.MonsterDict.TryGetValue(templatedId, out monsterData);
        if (monsterData == null) return null;

        TemplatedId = templatedId;
        switch(monsterData.monsterType)
        {
            case MonsterType.MONSTER_TYPE_CONTACT:
                break;
            case MonsterType.MONSTER_TYPE_RANGED:
                break;
            case MonsterType.MONSTER_TYPE_COUNTERATTACK:
                break;

        }

        return monster;
    }
}
