using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }

    public override void OnDamaged()
    {
        //Managers.Object.Remove(gameObject);
        Managers.Resource.Destroy(gameObject);
    }

}
