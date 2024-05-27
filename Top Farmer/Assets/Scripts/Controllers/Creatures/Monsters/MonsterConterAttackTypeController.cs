using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterConterAttackTypeController : MonsterController
{
    [SerializeField]
    bool _isAttacked = false;

    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            base.State = value;

            if (_coPatrol != null)
            {
                StopCoroutine(_coPatrol);
                _coPatrol = null;
            }
            if (_coSearch != null)
            {
                StopCoroutine(_coSearch);
                _coSearch = null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_monsterName == "Maggot")
        {
            if (_coPatrol == null)
            {
                _coPatrol = StartCoroutine("CoPatrol");
            }
            if (_coSearch == null)
            {
                _coSearch = StartCoroutine("CoSearch");
            }
        }
        else if (_monsterName == "EggCluster")
        {
            if (_coSearch == null)
            {
                _coSearch = StartCoroutine("CoSearch");
            }
            if (_target != null && _isAttacked)
            {
                Vector3Int dir = _target.GetComponent<CreatureController>().CellPos - CellPos;

                // 플레이어가 범위 내에 있고, 일직선상에 있을 때
                if (dir.magnitude <= Monster.SkillRange && (dir.x == 0 || dir.y == 0))
                {
                    Dir = GetDirFromVec(dir);
                    State = CreatureState.Skill;
                    _coSkill = StartCoroutine("CoStartAttack");
                    return;
                }
            }
        }

    }
    protected override void UpdateAnimation()
    {
        if(_monsterName == "Maggot")
        {
            base.UpdateAnimation();
        }
        else if(_monsterName == "EggCluster")
        {
            if (_state == CreatureState.Idle)
            {
                switch (_lastDir)
                {
                    case MoveDir.Up:
                        _animator.Play($"EggCluster_IDLE");
                        break;
                    case MoveDir.Down:
                        _animator.Play($"EggCluster_IDLE");
                        break;
                    case MoveDir.Left:
                        _animator.Play($"EggCluster_IDLE");
                        break;
                    case MoveDir.Right:
                        _animator.Play($"EggCluster_IDLE");
                        break;
                }
            }
            else if (_state == CreatureState.Moving)
            {
                switch (_lastDir)
                {
                    case MoveDir.Up:
                        _animator.Play($"EggCluster_IDLE");
                        break;
                    case MoveDir.Down:
                        _animator.Play($"EggCluster_IDLE");
                        break;
                    case MoveDir.Left:
                        _animator.Play($"EggCluster_IDLE");
                        break;
                    case MoveDir.Right:
                        _animator.Play($"EggCluster_IDLE");
                        break;
                }
            }
            else if (_state == CreatureState.Skill)
            {
                switch (_lastDir)
                {
                    case MoveDir.Up:
                        _animator.Play("EggCluster_ATTACK");
                        break;
                    case MoveDir.Down:
                        _animator.Play($"EggCluster_ATTACK");
                        break;
                    case MoveDir.Left:
                        _animator.Play($"EggCluster_ATTACK");
                        break;
                    case MoveDir.Right:
                        _animator.Play($"EggCluster_ATTACK");
                        break;
                    case MoveDir.None:
                        break;
                }
            }
        }
        
    }
    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;
        if (_target != null && _isAttacked)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;

            Vector3Int dir = destPos - CellPos;

            // 범위 내에 있고 일직선상에 있을 때
            if (dir.magnitude <= Monster.SkillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;
                _coSkill = StartCoroutine("CoStartAttack");
                return;
            }
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);

        // 길을 못찾았거나, (타겟이 있지만)너무 멀리있을 경우
        if (path.Count < 2 || (_target != null && path.Count > Monster.SearchRange))
        {
            _target = null;
            _isAttacked = false;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirFromVec(moveCellDir);

        if (Managers.Map.UpdateObjectPos(this.gameObject, (Vector2Int)nextPos))
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    public override void OnDamaged(int damage)
    {
        base.OnDamaged(damage);

        _isAttacked = true;
    }

    protected override IEnumerator CoStartAttack()
    {
        // 피격 판정
        GameObject go = Managers.Object.FindCreature(GetFrontCellPos());
        if (go != null)
        {
            PlayerController pc = go.GetComponent<PlayerController>();
            if (pc != null)
                pc.OnDamaged(Monster.Attack);
        }
        // 대기 시간
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;

         if(_monsterName == "EggCluster")
        {
            Managers.Object.RemoveMonster(gameObject);
            Managers.Resource.Destroy(gameObject);
        }
            
    }
}
