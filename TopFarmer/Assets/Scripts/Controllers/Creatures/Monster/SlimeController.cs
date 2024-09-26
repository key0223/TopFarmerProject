using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SlimeController : MonsterController
{
    [SerializeField] SpriteRenderer _faceSpriteRendere;

    protected override void Init()
    {
        base.Init();
        CellPos = GetGridPosition(transform.parent.position);
        _searchRange = 2f;
        _skillRange = 1f;
        //_faceSpriteRendere = transform.Find("Face").GetComponent<SpriteRenderer>();
    }
    protected override void UpdateMoving()
    {
        // destPos로 이동하고 도착하면 CellPos를 갱신
        if (_pathQueue.Count == 0)
        {
            State = CreatureState.Idle;
            return;
        }

        Vector3 destPos = GetWorldPosition(CellPos);
        Vector3 moveDir = destPos - transform.parent.position;
        float dist = moveDir.magnitude;

        if (dist < _speed * Time.deltaTime)
        {
            transform.parent.position = destPos;
            //CellPos = _pathQueue.Dequeue();

            if (_pathQueue.Count > 0)
            {
                MoveToNextPos();
            }
            else
            {
                State = CreatureState.Idle;
                return;
            }
        }
        else
        {
            transform.parent.position += moveDir.normalized * _speed * Time.deltaTime;
            Dir = GetDirFromVec(moveDir);
            State = CreatureState.Moving;
        }
    }
    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;

        if (_target != null)
        {
            destPos = GetGridPosition(_target.transform.position);

            Vector3Int dir = destPos - CellPos;
            if (dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;
                _coSkill = StartCoroutine("CoSkill");

                return;
            }
        }

        SetDestination(destPos);

        if (_pathQueue.Count < 2 || (_target != null && _pathQueue.Count > 10))
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        // 다음 목적지로 이동할 준비
        Vector3Int nextPos = _pathQueue.Dequeue();
        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirFromVec(moveCellDir);
        if (CanGo(nextPos))
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }
    public override IEnumerator CoSkill()
    {
        
    }
    #region State Controll
    protected override void UpdateAnimation()
    {
        if (_state == CreatureState.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    _faceSpriteRendere.enabled = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    _faceSpriteRendere.enabled = true;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    _faceSpriteRendere.enabled = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = true;
                    _faceSpriteRendere.enabled = true;
                    break;

            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    _faceSpriteRendere.enabled = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    _faceSpriteRendere.enabled = true;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    _faceSpriteRendere.enabled = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = true;
                    _faceSpriteRendere.enabled = true;
                    break;
            }
        }
        else if (_state == CreatureState.Skill)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.None:

                    break;
            }
        }
    }
    #endregion

}
