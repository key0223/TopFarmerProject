using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditorInternal;
using UnityEngine;
using static Define;

public class GhostController : MonsterController
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
        CellPos = GetGridPosition(transform.parent.position);

        #region Init Stat
        _maxHp = 96;
        _currentHp = _maxHp;
        _damage = 10;
        _defense = 0;
        _searchRange = 15f;
        _skillRange = 0f;
        _speed = 4f;
        _randomdurantionMovement = 9;
        //_dropItemDict = 
        _xp = 15;
        _displayName = "Ghost";
        #endregion
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
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_LEFT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_LEFT");
                    _sprite.flipX = true;
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
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_LEFT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_LEFT");
                    _sprite.flipX = true;
                    break;
            }
        }
        else if (_state == CreatureState.Dead)
        {

        }
    }
    #endregion

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_coSearch == null)
        {
            _coSearch = StartCoroutine(CoSearch());
        }
    }

    protected override void UpdateMoving()
    {
        if(_target != null)
        {
            Vector3 destPos =_target.transform.position;
            Vector3 moveDir = destPos - transform.parent.position;
            float dist = moveDir.magnitude;

            if (dist < _speed * Time.deltaTime)
            {
                transform.parent.position = destPos;
                State = CreatureState.Idle;
            }
            else
            {
                transform.parent.position += moveDir.normalized * _speed * Time.deltaTime;
                Dir = GetDirFromVec(moveDir);
                State = CreatureState.Moving;
            }
        }
        else
        {
            State = CreatureState.Idle;
        }
        
    }
    public override void OnDamaged(float damage, float knockbackDistance)
    {
        base.OnDamaged(damage, knockbackDistance);

        if (!_isKnockback)
        {
            Vector3 knockbackDirection = (transform.position - GetVecFromDir(_lastDir)).normalized;
            knockbackDirection.y = 0;

            StartCoroutine(CoKnckback(knockbackDirection, knockbackDistance));
        }

        damage -= _defense;
        _currentHp -= damage;
        if (_currentHp <= 0)
        {
            _currentHp = 0;
            OnDead();
        }
    }
    public override void OnDead()
    {
        base.OnDead();
        StartCoroutine(CoDead());
    }
    public override IEnumerator CoSearch()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();

        if (pc == null)
        {
            _target = null;
            yield return new WaitForSeconds(0.3f);
        }

        if (pc != null)
        {
            Vector2 dirToTarget = pc.transform.position - transform.parent.position;

            Vector2 currentDir = GetVecFromDir(Dir);

            if (Vector2.Angle(currentDir, dirToTarget) < _viewAngle)
            {
                if (dirToTarget.magnitude <= _searchRange)
                {
                    _target = pc.gameObject;
                    _destCellPos = GetGridPosition(_target.transform.position);
                    State = CreatureState.Moving;
                }
                else
                {
                    _target = null;
                   
                }
            }
            else
            {
                _target = null;
                
            }
        }
    }
    IEnumerator CoDead()
    {
        _animator.Play("DEAD");

        yield return new WaitForSeconds(GetAnimationClipLenth("DEAD"));

        Managers.Resource.Destroy(gameObject);
    }

    AnimationClip GetAnimationClip(string name)
    {
        foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        return null;
    }

    float GetAnimationClipLenth(string name)
    {
        AnimationClip clip = GetAnimationClip(name);

        if(clip != null)
        {
            float clipLength = clip.length;
            float clipSpeed = clip.apparentSpeed;

            float actualLength = clipLength / clipSpeed;

            return actualLength;
        }

        return 1;
    }
}
