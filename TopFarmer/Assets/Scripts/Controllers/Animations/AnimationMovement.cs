using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class AnimationMovement : MonoBehaviour
{
    protected Animator _animator;
    protected SpriteRenderer _sprite;
    [SerializeField]
    protected PlayerController _playerController;

    protected void Start()
    {
        Managers.Event.MovementEvent -= UpdateAnimation;
        Managers.Event.MovementEvent += UpdateAnimation;
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    public virtual void UpdateAnimation(CreatureState state, MoveDir dir, MoveDir lastDir, string itemType)
    {
        if (state == CreatureState.Idle)
        {
            switch (lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;

            }
        }
        else if (state == CreatureState.Moving)
        {
            switch (dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.None:

                    break;
            }
        }
        else if (state == CreatureState.ClickInput)
        {
            switch (lastDir)
            {
                case MoveDir.Up:
                    _animator.Play($"{itemType}_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play($"{itemType}_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play($"{itemType}_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play($"{itemType}_RIGHT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.None:

                    break;
            }
        }


    }
}
