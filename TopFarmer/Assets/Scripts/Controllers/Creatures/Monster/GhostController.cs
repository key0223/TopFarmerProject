using System.Collections;
using System.Collections.Generic;
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
        if (_target != null)
        {
            Vector3 destPos = _target.transform.position;
            Vector3 moveDir = destPos - transform.parent.position;
            float dist = moveDir.magnitude;

            // �巡�� ȿ���� ���� �ӵ� ����
            Vector3 velocity = moveDir.normalized * _speed;

            //if (dist <= 2f)
            //{
            //    float dragFactor = Mathf.Pow(dist / 2f, 2);  // �Ÿ��� �������� �ε巴�� ����
            //    velocity *= dragFactor;
            //}

            // ��ǥ ������ ����������� �ӵ� ���� (�巡�� ȿ��)
            float dragFactor = Mathf.Clamp01(dist / (_speed * 2f));  // �Ÿ��� �ӵ��� ���� �巡�� ���� ���
            velocity *= dragFactor;

            if (dist < velocity.magnitude * Time.deltaTime)
            {
                transform.parent.position = destPos;
                State = CreatureState.Idle;
            }
            else
            {
                transform.parent.position += velocity * Time.deltaTime;
                Dir = GetDirFromVec(moveDir);
                State = CreatureState.Moving;
            }
        }
        else
        {
            State = CreatureState.Idle;
        }

    }
    public override void OnDead()
    {
        base.OnDead();
        StartCoroutine(CoDead());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController pc = collision.gameObject.GetComponentInParent<PlayerController>();
            pc.OnDamaged(_damage);
        }
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
    public override IEnumerator CoKnockback(Vector3 direction )
    {
        _isKnockback = true;

        float elapsedTime = 0f;
        float drag = 2f; // �˹� �� ���� ����
        Vector3 currentVelocity = (direction * 10);



        while (elapsedTime < _knockbackDuration)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, drag * Time.deltaTime);

            // �˹� �̵� ����
            transform.parent.position += currentVelocity * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;  // �� ������ ���
        }

        // �˹��� ����Ǹ� ���� �ʱ�ȭ
        _isKnockback = false;
    }
    IEnumerator CoDead()
    {
        float remainingTime = GetRemainingAnimationTime();
        yield return new WaitForSeconds(remainingTime);

        _animator.Play("DEAD");

        yield return null;

        float clipLength = GetAnimationClipLenth("DEAD");
        yield return new WaitForSeconds(clipLength);

        DropItem();
        Managers.Resource.Destroy(transform.parent.gameObject);
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
    float GetRemainingAnimationTime(int layerIndex = 0)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(layerIndex);

        float clipLength = stateInfo.length;
        float normalizedTime = stateInfo.normalizedTime % 1f;

        // ���� �ð� ���
        float remainingTime = clipLength * (1f - normalizedTime);

        return remainingTime;
    }

    float GetAnimationClipLenth(string name)
    {
        AnimationClip clip = GetAnimationClip(name);

        if (clip != null)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] clipInfos = _animator.GetCurrentAnimatorClipInfo(0);

            foreach (AnimatorClipInfo clipInfo in clipInfos)
            {
                if (clipInfo.clip == clip)
                {
                    float clipLength = clip.length;  
                    float clipSpeed = stateInfo.speed * _animator.speed;  

                    float actualLength = clipLength / clipSpeed;

                    return actualLength;
                }
            }
        }
        return 1;
    }
}
