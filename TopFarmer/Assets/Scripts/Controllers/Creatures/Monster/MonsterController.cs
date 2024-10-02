using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngineInternal;
using static Define;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterController : CreatureController
{
    #region Monster Info
    protected float _maxHp;
    protected float _currentHp;
    protected int _damage;
    protected int _defense;
    protected bool _flayable;
    protected float _randomdurantionMovement;
    protected Dictionary<int, float> _dropItemDict = new Dictionary<int, float>();
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
    public GameObject Target { get { return _target; } }
    public float ViewAngle { get { return _viewAngle; } }
    public float SearchRange { get { return _searchRange; } }

    protected Coroutine _coSkill;
    protected Coroutine _coPatrol;
    protected Coroutine _coSearch;

    protected float _knockbackDuration = 0.2f;
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

        //_target = FindObjectOfType<PlayerController>().gameObject;
        // TODO : speed, ���� �ʱ�ȭ

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
            _coPatrol = StartCoroutine("CoPatrol");
        }

        if (_coSearch == null)
        {
            _coSearch = StartCoroutine("CoSearch");
        }
    }
    protected override void UpdateMoving()
    {
        // destPos�� �̵��ϰ� �����ϸ� CellPos�� ����
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


    public IEnumerator CoPatrol()
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

    public IEnumerator CoSearch()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();

        if (pc == null)
        {
            _target = null;
            yield break; // �ڷ�ƾ ����
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
    public override void OnDamaged(float damage,float knockbackDistance)
    {
        base.OnDamaged(damage,knockbackDistance);

        if(!_isKnockback)
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
        }
    }

    public virtual IEnumerator CoKnckback(Vector3 direction, float knockbackDistance)
    {
        _isKnockback = true;

        // �з����� �ð��� �������� �̵�
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = transform.position + direction * knockbackDistance;

        while (elapsedTime < _knockbackDuration)
        {
            // Lerp�� ����� ó�� ��ġ���� ��ǥ ��ġ�� �ε巴�� �̵�
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / _knockbackDuration);
            elapsedTime += Time.deltaTime;

            yield return null;  // ���� �����ӱ��� ���
        }

        // ������ ��ġ�� ��ǥ ��ġ�� ����
        transform.position = targetPosition;

        _isKnockback = false;  // �з����� ����
    }
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

            // �� �������� �̵� (���� ���⸸ ���)
            foreach (var next in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        // ��ǥ�� ������ �� ���� ��� �� ����Ʈ ��ȯ
        if (!cameFrom.ContainsKey(goal))
            return new List<Vector3Int>();


        // ��� ����
        List<Vector3Int> path = new List<Vector3Int>();
        for (Vector3Int current = goal; current != start; current = cameFrom[current])
        {
            path.Add(current);
        }
        path.Reverse(); // ��θ� ������ ��������� ��ǥ�� ���� ������ ��ȯ

        return path;
    }

    protected List<Vector3Int> GetNeighbors(Vector3Int current)
    {
        // �������� �̵� ������ �� ���� ��ȯ (��, ��, ��, ��)
        List<Vector3Int> neighbors = new List<Vector3Int>
 {
     current + new Vector3Int(1, 0, 0), // ������
     current + new Vector3Int(-1, 0, 0), // ����
     current + new Vector3Int(0, 1, 0), // ����
     current + new Vector3Int(0, -1, 0) // �Ʒ���
 };

        // �� ��ġ�� ��ȿ���� Ȯ���ϴ� ���� �߰� ���� (��: �� ���, ��ֹ� ��)

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
            angleInDegrees += transform.eulerAngles.z; // Z���� �������� ȸ��
        }
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
    }
    private void OnDrawGizmos()
    {
        if (_target == null)
            return;

        // �þ� ���� ���� ǥ�� (2D ȯ���, X-Y ��鿡�� �׸���)
        Gizmos.color = Color.white;
        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0);
        Gizmos.DrawWireSphere(position, _searchRange);

        // �þ߰� ǥ��
        Vector3 viewAngleA = DirFromAngle(-_viewAngle / 2, false);  // ���� �þ߰�
        Vector3 viewAngleB = DirFromAngle(_viewAngle / 2, false);   // ������ �þ߰�

        // �þ߰��� ������ ǥ�� (2D ���)
        Gizmos.DrawLine(position, transform.position + viewAngleA * _searchRange);
        Gizmos.DrawLine(position, transform.position + viewAngleB * _searchRange);

        // Ÿ���� ���̸� ���������� ���� �׸�
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
    private void OnValidate()
    {
        // �þ߰� ���� ����� ������ Scene �並 ����
        UnityEditor.SceneView.RepaintAll();
    }
}
