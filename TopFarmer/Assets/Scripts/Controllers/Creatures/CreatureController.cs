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


    public float _speed = 20f;
    protected Animator _animator;

    public CreatureType CreatureType { get; set; }

    [SerializeField]
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
    public MoveDir GetDirFromVec(Vector3 dir)
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
    public Vector3 GetVecFromDir(MoveDir moveDir)
    {
        Vector3 currentDir = Vector3.zero;
        switch (moveDir)
        {
            case MoveDir.Right:
                currentDir = Vector2.right;  
                break;
            case MoveDir.Left:
                currentDir = Vector2.left;  
                break;
            case MoveDir.Up:
                currentDir = Vector2.up;    
                break;
            case MoveDir.Down:
                currentDir = Vector2.down;   
                break;
        }

        return currentDir;
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
        else if (_state == CreatureState.ClickInput)
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
            case CreatureState.ClickInput:
                UpdateUsingTool();
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
    }
    protected virtual void MoveToNextPos()
    {
        

    }

    protected virtual void UpdateSkill()
    {

    }
    protected virtual void UpdateUsingTool()
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
