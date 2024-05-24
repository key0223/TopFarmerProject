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
   
    public static Monster MakeMonster(int templatedId)
    {
        Monster monster = null;
        MonsterData monsterData = null;
        Managers.Data.MonsterDict.TryGetValue(templatedId, out monsterData);
        if (monsterData == null) return null;

        monster = new Monster()
        {
            TemplatedId = monsterData.monsterId,
            Name = monsterData.name,
            ObjectType = monsterData.objectType,
            CreatureType = monsterData.creatureType,
            MonsterType = monsterData.monsterType,
            Level = monsterData.level,
            MaxHp = monsterData.maxHp,
            Attack = monsterData.attack,
            Speed = monsterData.speed,
            TotalExp = monsterData.totalExp,
        };

        return monster;
    }
}
