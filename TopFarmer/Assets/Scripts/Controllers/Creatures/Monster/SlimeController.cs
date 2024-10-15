using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using static Define;

[RequireComponent(typeof(MonsterStat))]
public class SlimeController : MonsterController
{
    [SerializeField] SpriteRenderer _faceSpriteRendere;
    Rigidbody2D _rigid;
    protected override void Init()
    {
        base.Init();
        CellPos = GetGridPosition(transform.parent.position);
        _rigid = GetComponentInParent<Rigidbody2D>();

        #region Init Stat
        _maxHp = 24;
        _currentHp = _maxHp;
        _damage = 5;
        _defense = 1;
        _searchRange = 5f;
        _skillRange = 1f;
        _speed = 3f;
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
        if (_target == null)
        {
            State = CreatureState.Idle;
            return;
        }


        Vector3 destPos = _target.transform.position;
        Vector3 moveDir = destPos - transform.parent.position;
        float dist = moveDir.magnitude;

        if (moveDir.magnitude <= _skillRange)
        {
            State = CreatureState.Skill;
            _coSkill = StartCoroutine(CoSkill()); // 스킬 발동
        }
        else
        {
            //transform.parent.position += moveDir.normalized * _speed * Time.deltaTime;
            _rigid.MovePosition(_rigid.position + (Vector2)moveDir * _speed * Time.deltaTime);
            Dir = GetDirFromVec(moveDir);
            State = CreatureState.Moving;
        }

    }

    public override void OnDead()
    {
        base.OnDead();
        Color monsterColor = _sprite.color;
        Managers.VFX.OnMonsterDeath(MonsterType.MONSTER_SLIME, transform.position, monsterColor);
        DropItem();
        Managers.Resource.Destroy(transform.parent.gameObject);
    }

    public override IEnumerator CoSearch()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();

        if (pc == null)
        {
            _target = null;
            yield break; // 코루틴 종료
        }

        _target = pc.gameObject; // 타겟을 캐싱

        if (_target != null)
        {
            Vector2 dirToTarget = (Vector2)_target.transform.position - _rigid.position;

            Vector2 currentDir = GetVecFromDir(Dir);

            if (Vector2.Angle(currentDir, dirToTarget) < _viewAngle && dirToTarget.magnitude <= _searchRange)
            {
                _target = pc.gameObject;
            }
            else
            {
                // 시야각을 벗어나면 초기화
                _target = null;
            }
        }
    }
    public override IEnumerator CoSkill()
    {
        Vector3 targetPos = _target.transform.position;
        Vector3 dir = _target.transform.position - (Vector3)_rigid.position;

        while (dir.magnitude > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(_rigid.position, targetPos, _speed * Time.deltaTime);
            _rigid.MovePosition(newPosition);

            dir = targetPos - -(Vector3)_rigid.position;

            Collider2D hit = Physics2D.OverlapCircle(_rigid.position, _skillRange, _mask);
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
    public override IEnumerator CoKnockback(Vector3 direction)
    {
        _isKnockback = true;

        float elapsedTime = 0f;
        float drag = 2f; // 넉백 중 감속 비율
        Vector3 currentVelocity = (direction * 10);

        while (elapsedTime < _knockbackDuration)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, drag * Time.deltaTime);

            // 넉백 이동 적용
            Vector2 newPosition = _rigid.position + (Vector2)(currentVelocity * Time.deltaTime);
            _rigid.MovePosition(newPosition);


            elapsedTime += Time.deltaTime;
            yield return null;  // 한 프레임 대기
        }

        // 넉백이 종료되면 상태 초기화
        _isKnockback = false;
    }
    protected void OnDrawGizmos()
    {
        if (_target == null)
            return;

        // 시야 범위 원형 표시 (2D 환경용, X-Y 평면에서 그리기)
        Gizmos.color = Color.white;
        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0);
        Gizmos.DrawWireSphere(position, _searchRange);

        // 시야각 표시
        Vector3 viewAngleA = DirFromAngle(-_viewAngle / 2, false);  // 왼쪽 시야각
        Vector3 viewAngleB = DirFromAngle(_viewAngle / 2, false);   // 오른쪽 시야각

        // 시야각을 선으로 표시 (2D 평면)
        Gizmos.DrawLine(position, _rigid.position + (Vector2)viewAngleA * _searchRange);
        Gizmos.DrawLine(position, _rigid.position + (Vector2)viewAngleB * _searchRange);

        // 타겟이 보이면 빨간색으로 선을 그림
        Gizmos.color = Color.red;

        Vector3 dir = (_target.transform.position - (Vector3)_rigid.position).normalized;
        float dist = (_target.transform.position - (Vector3)_rigid.position).magnitude;
        if (dist <= _searchRange && Vector3.Angle(GetVecFromDir(Dir), dir) < _viewAngle / 2)
        {
            Gizmos.DrawLine(_rigid.position, _target.transform.position);
        }
        //foreach (Transform visibleTarget in visibleTargets)
        //{
        //    Gizmos.DrawLine(transform.position, visibleTarget.position);
        //}
    }
    protected void OnValidate()
    {
        // 시야각 값이 변경될 때마다 Scene 뷰를 갱신
        UnityEditor.SceneView.RepaintAll();
    }
}
