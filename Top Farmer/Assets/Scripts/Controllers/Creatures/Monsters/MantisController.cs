using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MantisController : MonsterController
{
    [SerializeField]
    GameObject _target; // �����ϴ� ���
    float _searchRange = 5.0f;

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

        _speed = 3.0f;
    }
    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;
        if (_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);

        // ���� ��ã�Ұų�, (Ÿ���� ������)�ʹ� �ָ����� ���
        if (path.Count < 2 || (_target != null && path.Count > 10))
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        if (moveCellDir.x > 0)
            Dir = MoveDir.Right;
        else if (moveCellDir.x < 0)
            Dir = MoveDir.Left;
        else if (moveCellDir.y > 0)
            Dir = MoveDir.Up;
        else if (moveCellDir.y < 0)
            Dir = MoveDir.Down;
        else
            Dir = MoveDir.None;

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
                    _animator.Play("Mantis_MOVE_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play("Mantis_MOVE_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play("Mantis_MOVE_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play("Mantis_MOVE_RIGHT");
                    break;

            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("Mantis_MOVE_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play("Mantis_MOVE_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play("Mantis_MOVE_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play("Mantis_MOVE_RIGHT");
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
                    _animator.Play("Mantis_ATTACK_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play("Mantis_ATTACK_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play("Mantis_ATTACK_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play("Mantis_ATTACK_RIGHT");
                    break;
                case MoveDir.None:

                    break;
            }
        }
    }
}
