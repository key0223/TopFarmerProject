using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MaggotController : MonsterController
{

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
        else if (_state == CreatureState.UsingItem)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play("ATTACK_LEFT");
                    break;
                case MoveDir.None:

                    break;
            }
        }
        else
        {

        }
    }
}
