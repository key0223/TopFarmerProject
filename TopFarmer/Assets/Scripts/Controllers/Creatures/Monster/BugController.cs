using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BugController : MonsterController
{
    Vector3 _pointA;
    Vector3 _pointB;
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
        CellPos = GetGridPosition(transform.parent.position);

        #region Init Stat
        _maxHp = 1;
        _currentHp = _maxHp;
        _damage = 8;
        _defense = 0;
        _searchRange = 0f;
        _skillRange = 1f;
        _speed = 2f;
        _xp = 3;
        _displayName = "Bug";
        _pointA = transform.position;
        _pointB = GetRandomPoint();
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
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
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
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
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
    }
    #endregion

    protected override void UpdateIdle()
    {
        if (_coPatrol == null)
        {
            _coPatrol = StartCoroutine(CoPatrol());
        }
    }
    protected override void UpdateMoving()
    {

        // _destCellPos가 비어있으면 초기값 설정
        if (_destCellPos == Vector3Int.zero)
        {
            _destCellPos = GetGridPosition(_pointA); // 처음에는 _pointA로 이동
        }

        Vector3 destPos = GetWorldPosition(_destCellPos);
        Vector3 moveDir = destPos - transform.parent.position;
        float dist = moveDir.magnitude;

        // 목적지에 거의 도착한 경우
        if (dist < _speed * Time.deltaTime)
        {
            // 정확히 목적지 위치로 설정
            transform.parent.position = destPos;

            // 목적지가 _pointA이면 _pointB로, _pointB이면 _pointA로 이동
            if (_destCellPos == GetGridPosition(_pointA))
            {
                _destCellPos = GetGridPosition(_pointB); // _pointB로 이동
            }
            else
            {
                _destCellPos = GetGridPosition(_pointA); // _pointA로 이동
            }

            State = CreatureState.Idle; // 도착 후 잠시 멈춤
        }
        else
        {
            transform.parent.position += moveDir.normalized * _speed * Time.deltaTime;
            Dir = GetDirFromVec(moveDir);
            State = CreatureState.Moving;
        }
    }
    public override void OnDead()
    {
        State = CreatureState.Dead;
        Color monsterColor = _sprite.color;
        Managers.VFX.OnMonsterDeath(MonsterType.MONSTER_BUG, transform.position);
        DropItem();
        Managers.Resource.Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            pc.OnDamaged(_damage);
            Debug.Log("Bug Skill");
        }
    }
    Vector3 GetRandomPoint()
    {
        Vector3 point = Vector3.zero;

        int randDir = Random.Range(0, 5);
        int randDistance = Random.Range(0, 5);

        if (randDir == 0)
        {
            // vertical
            point = new Vector3(_pointA.x, (_pointA.y + randDistance), 0);
        }
        else
        {
            // horizontal
            point = new Vector3((_pointA.x + randDistance), _pointA.y, 0);
        }

        return point;
    }
    public override IEnumerator CoPatrol()
    {
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
    }

}
