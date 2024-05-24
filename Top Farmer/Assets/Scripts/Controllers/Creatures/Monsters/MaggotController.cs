using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static UnityEngine.GraphicsBuffer;

public class MaggotController : MonsterController
{

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
        }
    }

    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;

        _speed = 3.0f;
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_coPatrol == null)
        {
            _coPatrol = StartCoroutine("CoPatrol");
        }
    }
    protected override void MoveToNextPos()
    {
        Vector3Int moveCellPos = _destCellPos - CellPos;

        Dir = GetDirFromVec(moveCellPos);

        Vector3Int destPos = CellPos;

        switch (_dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        if (Managers.Map.UpdateObjectPos(this.gameObject, (Vector2Int)destPos))
        {
            CellPos = destPos;
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
                    _animator.Play("Maggot_MOVE_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play("Maggot_MOVE_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play("Maggot_MOVE_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play("Maggot_MOVE_RIGHT");
                    break;

            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("Maggot_MOVE_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play("Maggot_MOVE_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play("Maggot_MOVE_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play("Maggot_MOVE_RIGHT");
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
                    _animator.Play("Maggot_ATTACK_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play("Maggot_ATTACK_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play("Maggot_ATTACK_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play("Maggot_ATTACK_RIGHT");
                    break;
                case MoveDir.None:

                    break;
            }
        }
    }
}
