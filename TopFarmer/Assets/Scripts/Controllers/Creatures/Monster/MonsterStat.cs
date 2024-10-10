using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterStat : MonoBehaviour
{
    [SerializeField] int _monsterId;
    [SerializeField] string _name;
    [SerializeField] MonsterType _monsterType;
    [SerializeField] int _maxHp;
    [SerializeField] int _damage;
    [SerializeField] int _defense;
    [SerializeField] int _searchRange;
    [SerializeField] int _skillRange;
    [SerializeField] int _speed;
    [SerializeField] int _xp;
    [SerializeField] int _currentHp;


    public int MonsterId {get { return _monsterId;} private set { _monsterId = value; } }
    public string Name { get { return _name;} private set { _name = value; } }
    public MonsterType MonsterType { get { return _monsterType;} private set { _monsterType = value; } }
    public int MaxHp { get { return _maxHp;} private set { _maxHp = value; } }
    public int Damage { get { return _damage;} private set { _damage = value; } }
    public int Defense { get { return _defense;} private set { _defense = value; } }
    public int SearchRange { get { return _searchRange; } private set { _searchRange = value; } }
    public int SkillRange {  get { return _skillRange; } private set { _skillRange = value; } }
    public int Speed { get { return _speed; } private set { _speed = value; } } 
    public int Xp { get { return _xp; } private set { _xp = value; } }
    public int CurrentHp {  get { return _currentHp;} set { _currentHp = value; } }


    public void SetStat(int monsterId)
    {
        MonsterData monsterData = null;

        if(Managers.Data.MonsterDict.TryGetValue(monsterId, out monsterData))
        {
            _monsterId = monsterData.monsterId;
            _name = monsterData.name;
            _monsterType = monsterData.monsterType;
            _maxHp = monsterData.maxHp;
            _damage = monsterData.damage;
            _defense = monsterData.defense;
            _searchRange = monsterData.searchRange;
            _skillRange = monsterData.skillRange;
            _speed = monsterData.speed;
            _xp = monsterData.xp;
            CurrentHp = monsterData.maxHp;
        }
    }

}
