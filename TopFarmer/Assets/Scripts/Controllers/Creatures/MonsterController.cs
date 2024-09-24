using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Define;

public class MonsterController : CreatureController
{
    protected Grid _grid;
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    Vector3Int _destCellPos;

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

        // TODO : speed, 변수 초기화
         _speed = 3f;
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
        Vector3 destPos = GetWorldPosition(_destCellPos);
        Vector3 moveDir = destPos - transform.position;
        float dist = moveDir.magnitude;
        if(dist <_speed * Time.deltaTime)
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

    protected override void MoveToNextPos()
    {
        State = CreatureState.Idle;

    }

    public IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        int xRange = Random.Range(-2, 3);
        int yRange = Random.Range(-2, 3);
        Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

        _destCellPos = randPos;
        State = CreatureState.Moving;

    }

    public override void OnDamaged()
    {
        base.OnDamaged();
    }
}
