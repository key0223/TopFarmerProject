using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    public Monster Monster { get; set; }

    [SerializeField]
    public string _monsterName;
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

    [SerializeField]
    protected float _skillRange = 1.0f;
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

        Managers.Map.InitPos(gameObject, (Vector2Int)CellPos);
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }
    public virtual void SetStat()
    {
        _speed = Monster.Speed;
        _hp = Monster.MaxHp;

        // 수정 필요
        if (Monster.TemplatedId == 701)
        {
            _monsterName = "AcidBlob";
        }
        else if (Monster.TemplatedId == 711)
        {
            _monsterName = "Beetle";
        }
        else if (Monster.TemplatedId == 721)
        {
            _monsterName = "EggCluster";
        }
        else if (Monster.TemplatedId == 731)
        {
            _monsterName = "Maggot";
        }
        else if (Monster.TemplatedId == 741)
        {
            _monsterName = "Mantis";
        }

    }
    protected override void UpdateAnimation()
    {

        if (_state == CreatureState.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play($"{_monsterName}_MOVE_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play($"{_monsterName}_MOVE_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play($"{_monsterName}_MOVE_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play($"{_monsterName}_MOVE_RIGHT");
                    break;
            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play($"{_monsterName}_MOVE_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play($"{_monsterName}_MOVE_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play($"{_monsterName}_MOVE_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play($"{_monsterName}_MOVE_RIGHT");
                    break;
                case MoveDir.None:
                    break;
            }
        }
        else if (_state == CreatureState.Skill)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play($"{_monsterName}_ATTACK_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play($"{_monsterName}_ATTACK_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play($"{_monsterName}_ATTACK_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play($"{_monsterName}_ATTACK_RIGHT");
                    break;
                case MoveDir.None:
                    break;
            }
        }
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
            GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
            effect.transform.position = transform.position;
            effect.GetComponent<Animator>().Play("DieEffect");
            GameObject.Destroy(effect, 0.5f);

            Managers.Object.RemoveMonster(gameObject);
            Managers.Resource.Destroy(gameObject);
            
        }
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

    protected virtual IEnumerator CoSearch()
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
