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
                    Debug.Log("Idle Up");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("Maggot_MOVE_BACK");
                    Debug.Log("Idle Dwon");

                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("Maggot_MOVE_BACK");
                    Debug.Log("Idle Left");

                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("Maggot_MOVE_BACK");
                    Debug.Log("Idle Right");

                    _sprite.flipX = true;
                    break;

            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("Maggot_MOVE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("Maggot_MOVE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("Maggot_MOVE_LEFT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("Maggot_MOVE_RIGHT");
                    _sprite.flipX = false;
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
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("Maggot_ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("Maggot_ATTACK_LEFT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("Maggot_ATTACK_RIGHT");
                    _sprite.flipX = true;
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
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_LEFT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("ATTACK_LEFT");
                    _sprite.flipX = true;
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
