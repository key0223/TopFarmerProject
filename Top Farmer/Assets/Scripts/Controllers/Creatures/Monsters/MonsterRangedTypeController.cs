using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterRangedTypeController : MonsterController
{
 
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
        State = CreatureState.Idle;
        Dir = MoveDir.None;
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
}
