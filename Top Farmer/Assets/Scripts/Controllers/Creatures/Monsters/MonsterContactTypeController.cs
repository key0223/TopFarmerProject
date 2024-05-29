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

            // �÷��̾ ���� ���� �ְ�, �������� ���� ��
            if (dir.magnitude <= Monster.SkillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;
                _coSkill = StartCoroutine("CoStartAttack");
                return;
            }
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


    protected override IEnumerator CoStartAttack()
    {
        // �ǰ� ����
        GameObject go = Managers.Object.FindCreature(GetFrontCellPos());
        if (go != null)
        {
            PlayerController pc = go.GetComponent<PlayerController>();
            if (pc != null)
                pc.OnDamaged(Monster.Attack);
        }
        // ��� �ð�
        yield return new WaitForSeconds(0.5f);
        //State = CreatureState.Idle;
        _coSkill = null;

        State = CreatureState.Dead;

    }
}
