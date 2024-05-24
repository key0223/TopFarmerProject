using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    public Monster Monster { get; set; }
    public MonsterController()
    {
        ObjectType = ObjectType.OBJECT_TYPE_CREATURE;
    }
    protected Coroutine _coPatrol;
    protected Coroutine _coSearch;
    [SerializeField]
    protected Vector3Int _destCellPos;

    [SerializeField]
    GameObject _target; // 추적하는 대상
    float _searchRange = 5.0f;

    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            base.State = value;

            //if (_coPatrol != null)
            //{
            //    StopCoroutine(_coPatrol);
            //    _coPatrol = null;
            //}
            //if (_coSearch != null)
            //{
            //    StopCoroutine(_coSearch);
            //    _coSearch = null;
            //}
        }
    }
    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;

        //_speed = 3.0f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        //if (_coPatrol == null)
        //{
        //    _coPatrol = StartCoroutine("CoPatrol");
        //}
        //if (_coSearch == null)
        //{
        //    _coSearch = StartCoroutine("CoSearch");
        //}

    }
    /*
    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;
        if(_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos,destPos, ignoreDestCollision: true);
        
        // 길을 못찾았거나, (타겟이 있지만)너무 멀리있을 경우
        if(path.Count <2 || (_target != null && path.Count >10))
        {
            _target =null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        if (moveCellDir.x > 0)
            Dir = MoveDir.Right;
        else if (moveCellDir.x < 0)
            Dir = MoveDir.Left;
        else if (moveCellDir.y > 0)
            Dir = MoveDir.Up;
        else if (moveCellDir.y < 0)
            Dir = MoveDir.Down;
        else
            Dir = MoveDir.None;

        if (Managers.Map.UpdateObjectPos(this.gameObject, (Vector2Int)nextPos))
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }
    */
    public override void OnDamaged()
    {
        //Managers.Object.Remove(gameObject);
        Managers.Resource.Destroy(gameObject);
    }

    protected IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for (int i = 0; i < 4; i++)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);

            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if (Managers.Map.CanGo(randPos))
            {
                _destCellPos = randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle;

    }

    protected IEnumerator CoSearch()
    {

        // 1초마다 타겟을 찾는다.
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (_target != null)
                continue;

            _target = Managers.Object.FindCreature((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null)
                    return false;

                Vector3Int dir = (pc.CellPos - CellPos);
                if (dir.magnitude > _searchRange) return false;

                return true;

            });

        }
    }
}
