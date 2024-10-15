using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterObjective : Objective
{
    public string TargetName {  get; private set; } // TargetName indicates owner of the quest
    public int TargetMonsterId { get; private set; }

    public MonsterObjective(Quest quest)
    {
        MonsterQuest monsterQuest = quest as MonsterQuest;

        TargetName = monsterQuest.TargetName;
        TargetMonsterId = monsterQuest.TargetMonsterId;
        SuccessToComplete = monsterQuest.TargetQuantity;
        CurrentSuccess = 0;
        _objectiveAction = new IncrementObjectiveAction();

        Start();
        
    }

    public override void Start()
    {
        base.Start();
        Managers.Reporter.MonsterKilledEvent -= OnMonsterKilled;
        Managers.Reporter.MonsterKilledEvent += OnMonsterKilled;
    }

    public void OnMonsterKilled(int monsterId)
    {
        if (TargetMonsterId !=monsterId || IsComplete())
            return;

        ReceiveReport(1);

        if (CurrentSuccess >= SuccessToComplete)
        {
            ObjectiveState = Define.ObjectiveState.Complete;
            Managers.Reporter.MonsterKilledEvent -= OnMonsterKilled;
        }

    }
}
