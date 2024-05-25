using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterContactTypeController : MonsterController
{
    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            base.State = value;
           
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

        _speed = 0;
        _searchRange = 1;
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_coSearch == null)
        {
            _coSearch = StartCoroutine("CoSearch");
        }
        if (_target != null)
        {
            Vector3Int dir = _target.GetComponent<CreatureController>().CellPos - CellPos;

            // 플레이어가 범위 내에 있고, 일직선상에 있을 때
            if (dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;
                _coSkill = StartCoroutine("CoSplat");
                return;
            }
        }
    }
    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;
        if (_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;
            Vector3Int dir = destPos - CellPos;

            // 플레이어가 범위 내에 있고, 일직선상에 있을 때
            if (dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;
                _coSkill = StartCoroutine("CoSplat");
                return;
            }
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);

        // 길을 찾지 못했거나, 타겟이 너무 멀리 있을 경우
        if (path.Count < 2 || (_target != null && path.Count > 10))
        {
            _target = null;
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
    protected override void UpdateAnimation()
    {

        if (_state == CreatureState.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play($"AicdBlob1");
                    break;
                case MoveDir.Down:
                    _animator.Play($"AicdBlob1");
                    break;
                case MoveDir.Left:
                    _animator.Play($"AicdBlob1");
                    break;
                case MoveDir.Right:
                    _animator.Play($"AicdBlob1");
                    break;
            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play($"AicdBlob1");
                    break;
                case MoveDir.Down:
                    _animator.Play($"AicdBlob1");
                    break;
                case MoveDir.Left:
                    _animator.Play($"AicdBlob1");
                    break;
                case MoveDir.Right:
                    _animator.Play($"AicdBlob1");
                    break;
            }
        }
        else if (_state == CreatureState.Skill)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("AcidBlobSplat");
                    break;
                case MoveDir.Down:
                    _animator.Play($"AcidBlobSplat");
                    break;
                case MoveDir.Left:
                    _animator.Play($"AcidBlobSplat");
                    break;
                case MoveDir.Right:
                    _animator.Play($"AcidBlobSplat");
                    break;
                case MoveDir.None:
                    break;
            }
        }
    }


    IEnumerator CoSplat()
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

        Managers.Object.RemoveMonster(gameObject);
        Managers.Resource.Destroy(gameObject);
    }
    
}
