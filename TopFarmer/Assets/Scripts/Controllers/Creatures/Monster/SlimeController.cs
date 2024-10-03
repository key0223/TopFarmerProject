using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using static Define;

public class SlimeController : MonsterController
{
    [SerializeField] SpriteRenderer _faceSpriteRendere;

    protected override void Init()
    {
        base.Init();
        CellPos = GetGridPosition(transform.parent.position);

        #region Init Stat
        _maxHp = 24;
        _currentHp = _maxHp;
        _damage = 5;
        _defense = 1;
        _searchRange = 5f;
        _skillRange = 1f;
        _speed = 3f;
        _randomdurantionMovement = 9;
        //_dropItemDict = 
        _xp = 3;
        _displayName = "Green Slime";

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
    protected override void UpdateMoving()
    {
        // Move to the destPos and update CellPos upon arrival
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

            if (dir.magnitude <= _skillRange)
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

        // Prepare to move to the next destination
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
    public override void OnDead()
    {
        State = CreatureState.Dead;
        Color monsterColor = _sprite.color;
        Managers.VFX.OnMonsterDeath(MonsterType.MONSTER_SLIME, transform.position, monsterColor);
        Managers.Resource.Destroy(gameObject);
    }
   
    public override IEnumerator CoSkill()
    {
        Vector3 targetPos = _target.transform.position;
        Vector3 dir = _target.transform.position - transform.parent.position;

        while (dir.magnitude > 0.1f)
        {
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, targetPos, _speed * Time.deltaTime);

            dir = targetPos - transform.parent.position;

            Collider2D hit = Physics2D.OverlapCircle(transform.parent.position, _skillRange, _mask);
            if (hit != null && hit.gameObject.layer == (int)Layer.Player)
            {
                PlayerController pc = hit.gameObject.transform.parent.GetComponent<PlayerController>();
                pc.OnDamaged(_damage);
                Debug.Log("CoSkill Collider hit: " + hit.name);
                break;
            }

            yield return null;
        }


        yield return new WaitForSeconds(3f);
        State = CreatureState.Moving;
        _coSkill = null;

    }
    public override IEnumerator CoKnckback(Vector3 direction, float knockbackDistance)
    {
        _isKnockback = true;

        // 밀려나는 시간을 기준으로 이동
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.parent.position;
        Vector3 targetPosition = transform.parent.position + direction * knockbackDistance;

        while (elapsedTime < _knockbackDuration)
        {
            // Lerp를 사용해 처음 위치에서 목표 위치로 부드럽게 이동
            transform.parent.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / _knockbackDuration);
            elapsedTime += Time.deltaTime;

            yield return null;  // 다음 프레임까지 대기
        }

        // 마지막 위치를 목표 위치로 설정
        transform.parent.position = targetPosition;

        _isKnockback = false;  // 밀려남이 끝남
    }
   

}
