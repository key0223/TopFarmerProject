using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCPath))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class NPCMovement : MonoBehaviour
{
    public Define.Scene _npcCurrentScene;
    [HideInInspector] public Define.Scene _npcTargetScene;
    [HideInInspector] public Vector3Int _npcCurrentGridPosition;
    [HideInInspector] public Vector3Int _npcTargetGridPosition;
    [HideInInspector] public Vector3 _npcTargetWorldPosition;

    [SerializeField] CreatureState _state = CreatureState.Idle;
    public CreatureState State
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

    MoveDir _lastDir = MoveDir.Down;
    [SerializeField] MoveDir _dir;
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
   
    private Define.Scene _npcPreviousMovementStepScene;
    private Vector3Int _npcNextGridPosition;
    private Vector3 _npcNextWorldPosition;

    [Header("NPC Movement")]
    public float _npcNormalSpeed = 2f;

    [SerializeField] private float _npcMinSpeed = 1f;
    [SerializeField] private float _npcMaxSpeed = 3f;
    private bool _npcIsMoving = false;

    [HideInInspector] public AnimationClip _npcTargetAnimationClip;

    [Header("NPC Animation")]
    [SerializeField] private AnimationClip _blankAnimation = null;

    private Grid _grid;
    private Rigidbody2D _rigidBody2D;
    private BoxCollider2D _boxCollider2D;
    private WaitForFixedUpdate _waitForFixedUpdate;
    private Animator _animator;
    //private AnimatorOverrideController _animatorOverrideController;
    private int _lastMoveAnimationParameter;
    private NPCPath _npcPath;
    private bool _npcInitialised = false;
    private SpriteRenderer _sprite;
    [HideInInspector] public bool _npcActiveInScene = false;

    private bool _sceneLoaded = false;

    private Coroutine _moveToGridPositionRoutine;


    private void OnEnable()
    {
        Managers.Event.AfterSceneLoadEvent += AfterSceneLoad;
        Managers.Event.BeforeSceneUnloadEvent += BeforeSceneUnloaded;
    }

    private void OnDisable()
    {
        Managers.Event.AfterSceneLoadEvent -= AfterSceneLoad;
        Managers.Event.BeforeSceneUnloadEvent -= BeforeSceneUnloaded;
    }

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _npcPath = GetComponent<NPCPath>();
        _sprite = GetComponent<SpriteRenderer>();

        //_animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        //_animator.runtimeAnimatorController = _animatorOverrideController;

        // Initialise target world position, target _grid position & target scene to current
        _npcTargetScene = _npcCurrentScene;
        _npcTargetGridPosition = _npcCurrentGridPosition;
        _npcTargetWorldPosition = transform.position;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _waitForFixedUpdate = new WaitForFixedUpdate();

        SetIdleAnimation();
    }

    void UpdateAnimation()
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
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
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
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }

        else if (_state == CreatureState.Event)
        {

            switch (_lastDir)
            {
                case MoveDir.Up:
                    //_animator.Play("EVENT_ANIMATION");
                    _animator.Play($"{_npcTargetAnimationClip.name}");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    //_animator.Play("EVENT_ANIMATION");
                    _animator.Play($"{_npcTargetAnimationClip.name}");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    //_animator.Play("EVENT_ANIMATION");
                    _animator.Play($"{_npcTargetAnimationClip.name}");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    //_animator.Play("EVENT_ANIMATION");
                    _animator.Play($"{_npcTargetAnimationClip.name}");
                    _sprite.flipX = false;
                    break;
            }
        }
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

    private void FixedUpdate()
    {
        if (_sceneLoaded)
        {
            if (_npcIsMoving == false)
            {
                // set npc current and next _grid position - to take into account the npc might be animating
                _npcCurrentGridPosition = GetGridPosition(transform.position);
                _npcNextGridPosition = _npcCurrentGridPosition;

                if (_npcPath._npcMovementStepStack.Count > 0)
                {

                    NPCMovementStep npcMovementStep = _npcPath._npcMovementStepStack.Peek();

                    _npcCurrentScene = npcMovementStep._sceneName;

                    // If NPC is about the move to a new scene reset position to starting point in new scene and update the step times
                    if (_npcCurrentScene != _npcPreviousMovementStepScene)
                    {
                        _npcCurrentGridPosition = (Vector3Int)npcMovementStep._gridCoordinate;
                        _npcNextGridPosition = _npcCurrentGridPosition;
                        transform.position = GetWorldPosition(_npcCurrentGridPosition);
                        _npcPreviousMovementStepScene = _npcCurrentScene;
                        _npcPath.UpdateTimesOnPath();
                    }


                    // If NPC is in current scene then set NPC to active to make visible, pop the movement step off the stack and then call method to move NPC
                    if (_npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                    {
                        SetNPCActiveInScene();

                        npcMovementStep = _npcPath._npcMovementStepStack.Pop();

                        _npcNextGridPosition = (Vector3Int)npcMovementStep._gridCoordinate;

                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep._hour, npcMovementStep._minute, npcMovementStep._second);

                        MoveToGridPosition(_npcNextGridPosition, npcMovementStepTime, TimeManager.Instance.GetGameTime());
                    }

                    // else if NPC is not in current scene then set NPC to inactive to make invisible
                    // - once the movement step time is less than game time (in the past) then pop movement step off the stack and set NPC position to movement step position
                    else
                    {
                        SetNPCInactiveInScene();

                        _npcCurrentGridPosition = (Vector3Int)npcMovementStep._gridCoordinate;
                        _npcNextGridPosition = _npcCurrentGridPosition;
                        transform.position = GetWorldPosition(_npcCurrentGridPosition);

                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep._hour, npcMovementStep._minute, npcMovementStep._second);

                        TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                        if (npcMovementStepTime < gameTime)
                        {
                            npcMovementStep = _npcPath._npcMovementStepStack.Pop();

                            _npcCurrentGridPosition = (Vector3Int)npcMovementStep._gridCoordinate;
                            _npcNextGridPosition = _npcCurrentGridPosition;
                            transform.position = GetWorldPosition(_npcCurrentGridPosition);
                        }
                    }


                }
                // else if no more NPC movement steps
                else
                {
                    //ResetAnimation();

                    //SetNPCFacingDirection();

                    SetNPCEventAnimation();
                }
            }
        }
    }


    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        _npcTargetScene = npcScheduleEvent._toSceneName;
        _npcTargetGridPosition = (Vector3Int)npcScheduleEvent._toGridCoordinate;
        _npcTargetWorldPosition = GetWorldPosition(_npcTargetGridPosition);
        _dir = npcScheduleEvent._dir;
        _npcTargetAnimationClip = npcScheduleEvent._animationAtDestination;
        ClearNPCEventAnimation();
    }

    private void SetNPCEventAnimation()
    {
        if (_npcTargetAnimationClip != null)
        {
            //ResetAnimation();
            //_animatorOverrideController[_blankAnimation] = _npcTargetAnimationClip;


            State = CreatureState.Event;

            //_animator.SetBool(EventAnimation, true);
        }
        else
        {
            //_animatorOverrideController[_blankAnimation] = _blankAnimation;
            //_animator.SetBool(EventAnimation, false);
            State = CreatureState.Idle;
        }
    }

    public void ClearNPCEventAnimation()
    {
        //_animatorOverrideController[_blankAnimation] = _blankAnimation;
        //_animator.SetBool(EventAnimation, false);

        // Clear any rotation on npc
        transform.rotation = Quaternion.identity;
    }

    private void SetNPCFacingDirection()
    {
        //ResetAnimation();

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
                _animator.Play("IDLE_RIGHT");
                _sprite.flipX = false;
                break;

            case MoveDir.None:
                break;

            default:
                break;
        }
    }

    public void SetNPCActiveInScene()
    {
        _sprite.enabled = true;
        _boxCollider2D.enabled = true;
        _npcActiveInScene = true;
    }

    public void SetNPCInactiveInScene()
    {
        _sprite.enabled = false;
        _boxCollider2D.enabled = false;
        _npcActiveInScene = false;
    }

    private void AfterSceneLoad()
    {
        _grid = GameObject.FindObjectOfType<Grid>();

        if (!_npcInitialised)
        {
            InitialiseNPC();
            _npcInitialised = true;
        }

        _sceneLoaded = true;
    }

    private void BeforeSceneUnloaded()
    {
        _sceneLoaded = false;
    }

    /// <summary>
    /// returns the _grid position given the worldPosition
    /// </summary>
    private Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        if (_grid != null)
        {
            return _grid.WorldToCell(worldPosition);
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    /// <summary>
    ///  returns the world position (centre of _grid square) from gridPosition
    /// </summary>
    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = _grid.CellToWorld(gridPosition);

        // Get centre of _grid square
        return new Vector3(worldPosition.x + GridCellSize / 2f, worldPosition.y + GridCellSize / 2f, worldPosition.z);
    }

    public void CancelNPCMovement()
    {
        _npcPath.ClearPath();
        _npcNextGridPosition = Vector3Int.zero;
        _npcNextWorldPosition = Vector3.zero;

        _npcIsMoving = false;

        if (_moveToGridPositionRoutine != null)
        {
            StopCoroutine(_moveToGridPositionRoutine);
        }

        // Reset move animation
        //ResetAnimation();

        // Clear event animation
        ClearNPCEventAnimation();
        _npcTargetAnimationClip = null;

        //ResetAnimation();
    }


    private void InitialiseNPC()
    {
        // Active in scene
        if (_npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            SetNPCActiveInScene();
        }
        else
        {
            SetNPCInactiveInScene();
        }

        _npcPreviousMovementStepScene = _npcCurrentScene;

        // Get NPC Current Grid Position
        _npcCurrentGridPosition = GetGridPosition(transform.position);

        // Set Next Grid Position and Target Grid Position to current Grid Position
        _npcNextGridPosition = _npcCurrentGridPosition;
        _npcTargetGridPosition = _npcCurrentGridPosition;
        _npcTargetWorldPosition = GetWorldPosition(_npcTargetGridPosition);

        // Get NPC WorldPosition
        _npcNextWorldPosition = GetWorldPosition(_npcCurrentGridPosition);
    }

    private void MoveToGridPosition(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        _moveToGridPositionRoutine = StartCoroutine(MoveToGridPositionRoutine(gridPosition, npcMovementStepTime, gameTime));
    }

    // move NPC 
    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
      
        _npcIsMoving = true;

        SetMoveAnimation(gridPosition);

        _npcNextWorldPosition = GetWorldPosition(gridPosition);

        // If movement step time is in the future, otherwise skip and move NPC immediately to position
        if (npcMovementStepTime > gameTime)
        {
            //calculate time difference in seconds
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);

            // Calculate speed
            float npcCalculatedSpeed = Mathf.Max(_npcMinSpeed, Vector3.Distance(transform.position, _npcNextWorldPosition) / timeToMove / SecondsPerGameSecond);

            // If speed is at least npc min speed and less than npc max speed  then process, otherwise skip and move NPC immediately to position
            if (npcCalculatedSpeed <= _npcMaxSpeed)
            {
                while (Vector3.Distance(transform.position, _npcNextWorldPosition) > Define.PixelSize)
                {
                    Vector3 moveDir = Vector3.Normalize(_npcNextWorldPosition - transform.position);
                    Dir = GetDirFromVec(moveDir);

                    // 오브젝트가 한 프레임동안 이동할 거리 계산 
                    Vector2 move = new Vector2(moveDir.x * npcCalculatedSpeed * Time.fixedDeltaTime, moveDir.y * npcCalculatedSpeed * Time.fixedDeltaTime);


                    // 이동
                    _rigidBody2D.MovePosition(_rigidBody2D.position + move);
                    State = CreatureState.Moving;

                    yield return _waitForFixedUpdate;
                }
            }
        }
            
        _rigidBody2D.position = _npcNextWorldPosition;
        _npcCurrentGridPosition = gridPosition;
        _npcNextGridPosition = _npcCurrentGridPosition;

        _npcIsMoving = false;
        //State = CreatureState.Idle;
    }

    private void SetMoveAnimation(Vector3Int gridPosition)
    {
        //ResetAnimation();

        // get world position
        Vector3 toWorldPosition = GetWorldPosition(gridPosition);

        // get vector
        Vector3 directionVector = toWorldPosition - transform.position;

        Dir = GetDirFromVec(directionVector);
    }

    private void SetIdleAnimation()
    {
        _animator.Play("IDLE_FRONT");
    }

    void ResetAnimation()
    {
        _animator.Play("NONE");
    }

}
