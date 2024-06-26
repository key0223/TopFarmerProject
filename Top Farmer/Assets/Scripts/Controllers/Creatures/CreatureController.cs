using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : ObjectController
{
    // 피격
    protected float _delay = 0.1f;
    protected int _repeat = 4;
    protected int _repeatCount = 0;


    [SerializeField]
    public float _speed = 5f;
    protected Animator _animator;

    public CreatureType CreatureType { get; set; }

    protected CreatureState _state = CreatureState.Idle;
    public virtual CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
            UpdateAnimation();
        }
    }
    protected MoveDir _lastDir = MoveDir.Down;
    [SerializeField]
    protected MoveDir _dir = MoveDir.Down;
    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;

            _dir = value;
            if (value != MoveDir.None)
                _lastDir = value;

            UpdateAnimation();
        }
    }

    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else if (dir.y < 0)
            return MoveDir.Down;
        else
            return MoveDir.None;
    }
    public Vector3Int GetFrontCellPos(int range = 1)
    {
        Vector3Int cellPos = CellPos;

        switch (_lastDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up * range;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down * range;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left * range;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right * range;
                break;
        }
        return cellPos;
    }

    #region State Controll
    protected virtual void UpdateAnimation()
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
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_LEFT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_LEFT");
                    _sprite.flipX = true;
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

    void Update()
    {
        UpdateController();
    }

    protected override void Init()
    {
        base.Init();
        Vector2Int initPos = new Vector2Int(0, 0);
        if (Managers.Map.UpdateObjectPos(gameObject, initPos))
        {
            CellPos = new Vector3Int(0, 0);

        }
        _animator = GetComponent<Animator>();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.UsingItem:
                UpdateUsingItem();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }
    // 이동 가능한 상태일 때, 실제 좌표 이동
    protected virtual void UpdateIdle()
    {

    }
    // 스르륵 이동
    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }

    }
    protected virtual void MoveToNextPos()
    {
        if (_dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }
        Vector3Int destPos = CellPos;

        switch (_dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        if (Managers.Map.UpdateObjectPos(this.gameObject, (Vector2Int)destPos))
        {
            CellPos = destPos;
        }

    }

    protected virtual void UpdateSkill()
    {

    }
    protected virtual void UpdateUsingItem()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    public virtual void OnDamaged(int damage)
    {
        _repeatCount = _repeat;
        StartCoroutine(CoFlicker());
    }

    #endregion

    public IEnumerator CoFlicker()
    {
        while (_repeatCount > 0)
        {
            _repeatCount--;

            _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 0.5f);
            yield return new WaitForSeconds(_delay);

            _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 1f);
            yield return new WaitForSeconds(_delay);
        }
    }
}
