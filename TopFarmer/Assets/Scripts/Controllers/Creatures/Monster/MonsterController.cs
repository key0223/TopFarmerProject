using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngineInternal;
using static Define;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterController : CreatureController
{
    MonsterStat _stat;

    #region Monster Info
    protected float _maxHp;
    protected float _currentHp;
    protected int _damage;
    protected int _defense;
    protected bool _flayable;
    protected int _xp;
    protected string _displayName;
    #endregion


    protected Grid _grid;
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    protected Vector3Int _destCellPos;
    protected Queue<Vector3Int> _pathQueue = new Queue<Vector3Int>();

    [Header("Target")]
    [SerializeField] protected GameObject _target;
    [SerializeField] protected float _viewAngle;
    [SerializeField] protected float _searchRange;
    [SerializeField] protected float _skillRange;

    protected int _mask = (1 << (int)Layer.Wall | (1 << (int)Layer.Player));
  
    protected Coroutine _coSkill;
    protected Coroutine _coPatrol;
    protected Coroutine _coSearch;

    protected float _knockbackDuration = 0.5f;
    protected bool _isKnockback = false;
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
        _grid = GameObject.FindObjectOfType<Grid>();
        State = CreatureState.Idle;
        Dir = MoveDir.None;
        _stat = gameObject.GetComponent<MonsterStat>();

        //_target = FindObjectOfType<PlayerController>().gameObject;
        // TODO : speed, 변수 초기화

        _speed = 3f;
        _currentHp = _maxHp;
        CellPos = GetGridPosition(transform.position);
    }
    protected Vector3Int GetGridPosition(Vector3 worldPosition)
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
    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = _grid.CellToWorld(gridPosition);

        // Get centre of _grid square
        return new Vector3(worldPosition.x + GridCellSize / 2f, worldPosition.y + GridCellSize / 2f, worldPosition.z);
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        if (_coPatrol == null)
        {
            _coPatrol = StartCoroutine(CoPatrol());
        }

        if (_coSearch == null)
        {
            _coSearch = StartCoroutine(CoSearch());
        }
    }
    protected override void UpdateMoving()
    {
        // destPos로 이동하고 도착하면 CellPos를 갱신
        if (_pathQueue.Count == 0)
        {
            State = CreatureState.Idle;
            return;
        }

        Vector3 destPos = GetWorldPosition(CellPos);
        Vector3 moveDir = destPos - transform.position;
        float dist = moveDir.magnitude;

        Dir = GetDirFromVec(moveDir);

        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            CellPos = _pathQueue.Dequeue();

            if (_pathQueue.Count > 0)
            {
                MoveToNextPos();
            }
            else
            {
                State = CreatureState.Idle;
                return;
            }
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            Dir = GetDirFromVec(moveDir);
            State = CreatureState.Moving;
        }
    }

   
    protected override void MoveToNextPos()
    {

        if (_pathQueue.Count < 2 || _pathQueue.Count > 10)
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        // Prepare to move to the next position
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
    public override void OnDamaged(float damage, float knockbackDistance, Vector3 collPos)
    {
        base.OnDamaged(damage, knockbackDistance);

        if (!_isKnockback)
        {
            Vector3 knockbackDirection = (transform.position - collPos).normalized;

            ApplyKnockback(knockbackDirection, knockbackDistance);
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
    }

    protected  void DropItem()
    {
        List<int> dropItems = GetDropItems();

        foreach (int item in dropItems)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f), 
                                                transform.position.y + Random.Range(-1f, 1f), 0f);
            SceneItemsManager.Instance.InstantiateSceneItems(item, spawnPosition);
        }
    }
    protected List<int> GetDropItems()
    {
        List<int> dropItems = new List<int>();

        int dropCount = Random.Range(1, 3);

        for (int i = 0; i < dropCount; i++)
        {
            float randomValue = Random.Range(0f, 1f);
            float cumulativeProbability = 0f;

            foreach (var item in Managers.Data.MonsterDict[_stat.MonsterId].dropTable)
            {
                cumulativeProbability += item.Value;
                if (randomValue <= cumulativeProbability)
                {
                    dropItems.Add(item.Key);
                    break; 
                }
            }
        }

        // if dropItems is empty
        if (dropItems.Count == 0)
        {
            float randomValue = Random.Range(0f, 1f);
            float cumulativeProbability = 0f;

            foreach (var item in Managers.Data.MonsterDict[_stat.MonsterId].dropTable)
            {
                cumulativeProbability += item.Value;
                if (randomValue <= cumulativeProbability)
                {
                    dropItems.Add(item.Key);
                    break;
                }
            }
        }

        return dropItems;
    }

    #region Actions
    public virtual IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for (int i = 0; i < 10; i++)
        {
            int xRange = Random.Range(-2, 3);
            int yRange = Random.Range(-2, 3);

            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if (CanGo(randPos))
            {
                _destCellPos = randPos;
                SetDestination(_destCellPos);
                State = CreatureState.Moving;
                yield break;
            }

        }
        State = CreatureState.Idle;

        //yield return new WaitForSeconds(Random.Range(2, 5));
    }

    public virtual IEnumerator CoSearch()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();

        if (pc == null)
        {
            _target = null;
            yield break; // 코루틴 종료
        }

        if (pc != null)
        {
            Vector2 dirToTarget = pc.transform.position - transform.position;

            Vector2 currentDir = GetVecFromDir(Dir);

            if (Vector2.Angle(currentDir, dirToTarget) < _viewAngle)
            {
                if (dirToTarget.magnitude <= _searchRange)
                {
                    _target = pc.gameObject;
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

    public virtual IEnumerator CoSkill()
    {
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Moving;
        _coSkill = null;
    }
    public void ApplyKnockback(Vector3 direction, float knockbackDistance)
    {
        if (_isKnockback)
            return;

        direction = direction.normalized * knockbackDistance;
        StartCoroutine(CoKnockback(direction));  // 코루틴 시작
    }

    public virtual IEnumerator CoKnockback(Vector3 direction)
    {
        _isKnockback = true;


        float elapsedTime = 0f;
        float drag = 2f; // 넉백 중 감속 비율
        Vector3 currentVelocity = direction;



        while (elapsedTime < _knockbackDuration)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, drag * Time.deltaTime);

            // 넉백 이동 적용
            transform.position += currentVelocity * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;  // 한 프레임 대기
        }

        // 넉백이 종료되면 상태 초기화
        _isKnockback = false;
    }
    #endregion

    #region Path
    public void SetDestination(Vector3Int destPos)
    {
        _pathQueue.Clear();
        List<Vector3Int> path = FindPath(CellPos, destPos);
        if (path != null && path.Count > 0)
        {
            _pathQueue = new Queue<Vector3Int>(path);
            State = CreatureState.Moving;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }
    protected List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        Queue<Vector3Int> frontier = new Queue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        frontier.Enqueue(start);
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();

            if (current == goal)
                break;

            // 네 방향으로 이동 (직선 방향만 고려)
            foreach (var next in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        // 목표에 도달할 수 없을 경우 빈 리스트 반환
        if (!cameFrom.ContainsKey(goal))
            return new List<Vector3Int>();


        // 경로 복원
        List<Vector3Int> path = new List<Vector3Int>();
        for (Vector3Int current = goal; current != start; current = cameFrom[current])
        {
            path.Add(current);
        }
        path.Reverse(); // 경로를 뒤집어 출발지에서 목표로 가는 순서로 반환

        return path;
    }

    protected List<Vector3Int> GetNeighbors(Vector3Int current)
    {
        // 직선으로 이동 가능한 네 방향 반환 (상, 하, 좌, 우)
        List<Vector3Int> neighbors = new List<Vector3Int>
 {
     current + new Vector3Int(1, 0, 0), // 오른쪽
     current + new Vector3Int(-1, 0, 0), // 왼쪽
     current + new Vector3Int(0, 1, 0), // 위쪽
     current + new Vector3Int(0, -1, 0) // 아래쪽
 };

        // 각 위치가 유효한지 확인하는 로직 추가 가능 (예: 맵 경계, 장애물 등)

        return neighbors;
    }
    #endregion
   
    protected bool CanGo(Vector3Int targetPosition)
    {

        Vector3 direction = (targetPosition - Vector3Int.FloorToInt(transform.position));
        Vector3 normalizedDir = direction.normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, normalizedDir, 1f, _mask);


        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z; // Z축을 기준으로 회전
        }
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
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
        Gizmos.DrawLine(position, transform.position + viewAngleA * _searchRange);
        Gizmos.DrawLine(position, transform.position + viewAngleB * _searchRange);

        // 타겟이 보이면 빨간색으로 선을 그림
        Gizmos.color = Color.red;

        float dist = (_target.transform.position - transform.position).magnitude;
        if (dist <= _searchRange)
        {
            Gizmos.DrawLine(transform.position, _target.transform.position);
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
