using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterInfo
{
    public int templatedId;
    public string name;

}
public class Monster : MonoBehaviour
{
    public MonsterInfo Info { get; } = new MonsterInfo();
    public StatInfo Stat { get; } = new StatInfo();

    public MonsterType MonsterType { get; private set; }

    public int TemplatedId
    {
        get { return Info.templatedId; }
        set { Info.templatedId = value; }
    }
    public string Name
    {
        get { return Info.name; }
        set { Info.name = value; }
    }
    public int Level
    {
        get { return Stat.level; }
        set { Stat.level = value; }
    }
    public int Hp
    {
        get { return Stat.hp; }
        set { Stat.hp = value; }
    }
    public int MaxHp
    {
        get { return Stat.maxHp; }
        set { Stat.maxHp = value; }
    }
    public int Attack
    {
        get { return Stat.attack; }
        set { Stat.attack = value; }
    }
    public float Speed
    {
        get { return Stat.speed; }
        set { Stat.speed = value; }
    }
    public int TotalExp
    {
        get { return Stat.totalExp; }
        set { Stat.totalExp = value; }

    }


    public void Init(int templatedId)
    {
        TemplatedId = templatedId;

        MonsterData monsterData = null;
        Managers.Data.MonsterDict.TryGetValue(TemplatedId, out monsterData);
        Name = monsterData.name;
        MonsterType = monsterData.monsterType;

        Level = monsterData.level;
        Hp = monsterData.maxHp;
        MaxHp= monsterData.maxHp;
        Attack = monsterData.attack;
        Speed = monsterData.speed;
        TotalExp = monsterData.totalExp;

    }
}
