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

    public Monster(MonsterType monsterType)
    {
        MonsterType = monsterType;
    }

    public void Init(int templatedId)
    {
        TemplatedId = templatedId;

        MonsterData monsterData = null;

    }
}
