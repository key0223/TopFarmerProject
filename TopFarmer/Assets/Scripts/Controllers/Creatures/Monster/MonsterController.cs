using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Define;

public class MonsterController : CreatureController
{
    protected Grid _grid;
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    protected Vector3Int _destCellPos;
    protected Queue<Vector3Int> _pathQueue = new Queue<Vector3Int>();

    [Header("Target")]
    [SerializeField] protected GameObject _target;
    [SerializeField] float _viewAngle;
    [SerializeField] protected float _searchRange = 10f;
    [SerializeField] protected float _skillRange = 1.0f;

    public GameObject Target { get { return _target; } }
    public float ViewAngle { get { return _viewAngle; } }
    public float SearchRange { get { return  _searchRange; } }

    protected Coroutine _coSkill;
    protected Coroutine _coPatrol;
    protected Coroutine _coSearch;
    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if(_state == value)
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

        if (_pathQueue.Count < 2 ||_pathQueue.Count > 10)
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        // ���� �������� �̵��� �غ�
        Vector3Int nextPos = _pathQueue.Dequeue();
        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirFromVec(moveCellDir);
        CellPos = nextPos;
    }


    public IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 8);
        yield return new WaitForSeconds(waitSeconds);

        int xRange = Random.Range(-2, 3);
        int yRange = Random.Range(-2, 3);

        Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

        _destCellPos = randPos;
        SetDestination(_destCellPos);
        State = CreatureState.Moving;

        yield return new WaitForSeconds(Random.Range(2, 5));
    }

    public IEnumerator CoSearch()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (_target != null)
                continue;

            PlayerController foundTarget = FindObjectsOfType<PlayerController>().FirstOrDefault(pc =>
            {
                Vector3Int dir = GetGridPosition(pc.transform.position) - CellPos;

                if (dir.magnitude > _searchRange)
                    return false;

                return true;
            });  

            if (foundTarget != null)
            {
                _target = foundTarget.gameObject;  
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
        Debug.Log("Skill");
        State = CreatureState.Moving;
        _coSkill = null;
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
    public override void OnDamaged()
    {
        base.OnDamaged();
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z; // Z���� �������� ȸ��
        }
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }
    /*
    private void OnDrawGizmos()
    {
        // �þ� ���� ���� ǥ�� (2D ȯ���, X-Y ��鿡�� �׸���)
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _searchRange);

        // �þ߰� ǥ��
        Vector3 viewAngleA = DirFromAngle(-_viewAngle / 2, false);  // ���� �þ߰�
        Vector3 viewAngleB = DirFromAngle(_viewAngle / 2, false);   // ������ �þ߰�

        // �þ߰��� ������ ǥ�� (2D ���)
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * _searchRange);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * _searchRange);

        // Ÿ���� ���̸� ���������� ���� �׸�
        Gizmos.color = Color.red;

        float dist = (_target.transform.position - transform.position).magnitude;
        if (dist<= _searchRange)
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
    */
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            Debug.Log(collision.gameObject.name);
        }
    }
}
