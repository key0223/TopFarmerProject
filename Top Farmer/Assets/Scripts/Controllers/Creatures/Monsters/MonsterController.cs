using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    public Monster Monster { get; set; }

    [SerializeField]
    public string _monsterName;
    public MonsterController()
    {
        ObjectType = ObjectType.OBJECT_TYPE_CREATURE;
        CreatureType = CreatureType.CREATURE_TYPE_MONSTER;
    }
    protected Coroutine _coPatrol;
    protected Coroutine _coSearch;
    protected Coroutine _coSkill;

    [SerializeField]
    protected Vector3Int _destCellPos;

    [SerializeField]
    protected GameObject _target; // 추적하는 대상

    protected int _hp;


    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            base.State = value;
        }
    }
    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;

        Managers.Map.InitPos(gameObject, (Vector2Int)CellPos);
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

    }
    public virtual void SetStat()
    {
        _speed = Monster.Speed;
        _hp = Monster.MaxHp;

        // 수정 필요
        if (Monster.TemplatedId == 701)
        {
            _monsterName = "AcidBlob";
        }
        else if (Monster.TemplatedId == 711)
        {
            _monsterName = "Beetle";
        }
        else if (Monster.TemplatedId == 721)
        {
            _monsterName = "EggCluster";
        }
        else if (Monster.TemplatedId == 731)
        {
            _monsterName = "Maggot";
        }
        else if (Monster.TemplatedId == 741)
        {
            _monsterName = "Mantis";
        }

    }

    protected override void UpdateAnimation()
    {

        if (_state == CreatureState.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play($"{_monsterName}_MOVE_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play($"{_monsterName}_MOVE_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play($"{_monsterName}_MOVE_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play($"{_monsterName}_MOVE_RIGHT");
                    break;
            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play($"{_monsterName}_MOVE_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play($"{_monsterName}_MOVE_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play($"{_monsterName}_MOVE_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play($"{_monsterName}_MOVE_RIGHT");
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
                    _animator.Play($"{_monsterName}_ATTACK_BACK");
                    break;
                case MoveDir.Down:
                    _animator.Play($"{_monsterName}_ATTACK_FRONT");
                    break;
                case MoveDir.Left:
                    _animator.Play($"{_monsterName}_ATTACK_LEFT");
                    break;
                case MoveDir.Right:
                    _animator.Play($"{_monsterName}_ATTACK_RIGHT");
                    break;
                case MoveDir.None:
                    break;
            }
        }
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }
    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;
        if (_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;

            Vector3Int dir = destPos - CellPos;

            // 범위 내에 있고 일직선상에 있을 때
            if (dir.magnitude <= Monster.SkillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;
                _coSkill = StartCoroutine("CoStartAttack");
                return;
            }
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);

        // 길을 못찾았거나, (타겟이 있지만)너무 멀리있을 경우
        if (path.Count < 2 || (_target != null && path.Count > 10))
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirFromVec(moveCellDir);

        if (Managers.Map.UpdateObjectPos(this.gameObject, (Vector2Int)nextPos))
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }
    public override void OnDamaged(int damage)
    {
        base.OnDamaged(damage);

        _hp = Mathf.Max(_hp - damage, 0);
        Debug.Log($"Monster Hp : {_hp}");

        if(_hp <= 0)
        {

            GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
            effect.transform.position = transform.position;
            effect.GetComponent<Animator>().Play("DieEffect");
            GameObject.Destroy(effect, 0.5f);

            State = CreatureState.Dead;

            //Managers.Object.RemoveMonster(gameObject);
            //Managers.Resource.Destroy(gameObject);

        }
    }

    protected override void UpdateDead()
    {
        RewardData rewardData = GetRandomReward();
        if(rewardData != null)
        {
            int randCount = Random.Range(1, rewardData.count);

            GameObject dropItem = Managers.Resource.Instantiate($"Object/Craftable/Interactable/DropItem");
            ItemDrop drop = dropItem.GetComponent<ItemDrop>();
            drop.Init(rewardData.itemId, randCount);
            drop.OnDropItem(CellPos);

            State = CreatureState.Idle;
            // TODO: 아이템 드랍
        }
        Managers.Object.RemoveMonster(gameObject);
        Managers.Resource.Destroy(gameObject);
    }
    RewardData GetRandomReward()
    {
        List<RewardData> data = null;
        Managers.Data.RewardDict.TryGetValue(Monster.TemplatedId, out data);

        int rand = Random.Range(0, 101);
        int sum = 0;
        foreach(RewardData rewardData in data)
        {
            sum += rewardData.probability;
            if(rand<= sum)
            {
                return rewardData;
            }
        }

        return null;
    }

    #region Coroutine
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

    protected  IEnumerator CoSearch()
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
                if (dir.magnitude > Monster.SearchRange) return false;

                return true;

            });
        }
    }

    protected virtual IEnumerator CoStartAttack()
    {
        // 피격 판정
        GameObject go = Managers.Object.FindCreature(GetFrontCellPos());
        if (go != null)
        {
            PlayerController pc = go.GetComponent<PlayerController> ();
            if (pc != null)
                pc.OnDamaged(Monster.Attack);
        }
        // 대기 시간
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
        _coSkill = null;
    }
    #endregion
}
