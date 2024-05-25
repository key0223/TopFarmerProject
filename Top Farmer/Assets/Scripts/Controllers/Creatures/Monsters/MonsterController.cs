using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    public Monster Monster { get; set; }
    public MonsterController()
    {
        ObjectType = ObjectType.OBJECT_TYPE_CREATURE;
        CreatureType = CreatureType.CREATURE_TYPE_MONSTER;
    }
    protected Coroutine _coPatrol;
    protected Coroutine _coSearch;
    protected Coroutine _coSkill;
    [SerializeField]
    protected Vector3Int _destCellPos;

    [SerializeField]
    protected GameObject _target; // 추적하는 대상

    protected int _hp;


    protected float _searchRange = 5.0f;

    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            base.State = value;
        }
    }
    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }
    public virtual void SetStat()
    {
        _speed = Monster.Speed;
        _hp = Monster.MaxHp;

    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }
    public override void OnDamaged(int damage)
    {
        base.OnDamaged(damage);

        _hp = Mathf.Max(_hp - damage, 0);
        Debug.Log($"Monster Hp : {_hp}");

        if(_hp <= 0)
        {
            Managers.Object.Remove(gameObject);
            Managers.Resource.Destroy(gameObject);
            
        }
        //Managers.Object.Remove(gameObject);
    }

    protected IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for (int i = 0; i < 4; i++)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);

            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if (Managers.Map.CanGo(randPos))
            {
                _destCellPos = randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle;

    }

    protected IEnumerator CoSearch()
    {
        // 1초마다 타겟을 찾는다.
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (_target != null)
                continue;

            _target = Managers.Object.FindCreature((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null)
                    return false;

                Vector3Int dir = (pc.CellPos - CellPos);
                if (dir.magnitude > _searchRange) return false;

                return true;

            });
        }
    }

    protected IEnumerator CoStartPunch()
    {
        // 피격 판정
        GameObject go = Managers.Object.FindCreature(GetFrontCellPos());
        if (go != null)
        {
            PlayerController pc = go.GetComponent<PlayerController> ();
            if (pc != null)
                pc.OnDamaged(Monster.Attack);
        }
        // 대기 시간
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
}
