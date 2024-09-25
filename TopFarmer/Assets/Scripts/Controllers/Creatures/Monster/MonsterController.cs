using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Define;

public class MonsterController : CreatureController
{
    protected Grid _grid;
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    protected Vector3Int _destCellPos;
    protected Queue<Vector3Int> _pathQueue = new Queue<Vector3Int>();

    Coroutine _coSkill;
    Coroutine _coPatrol;
    Coroutine _coSearch;
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
        }
    }

    protected override void Init()
    {
        base.Init();
        _grid = GameObject.FindObjectOfType<Grid>();
        State = CreatureState.Idle;
        Dir = MoveDir.None;

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
            State = CreatureState.Idle;
            return;
        }

        // ���� �������� �̵��� �غ�
        Vector3Int nextPos = _pathQueue.Dequeue();
        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirFromVec(moveCellDir);
        _destCellPos = nextPos;
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
        //State = CreatureState.Moving;

        yield return new WaitForSeconds(Random.Range(2, 5));
    }

    #region Path
    public void SetDestination(Vector3Int destPos)
    {
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
}
