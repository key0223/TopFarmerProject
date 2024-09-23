using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocializeObjective : Objective
{
    public string[] TargetNames { get; private set; }

    List<string> _contactedNPC =new List<string>();

    public SocializeObjective(Quest quest)
    {
        SocializeQuest socializeQuest = (SocializeQuest)quest;
        TargetNames = socializeQuest.TargetNames;
        SuccessToComplete = socializeQuest.TargetCount;
        CurrentSuccess = 0;
        _objectiveAction = new IncrementObjectiveAction();

        Start();
    }

    public override void Start()
    {
        base.Start();
        Managers.Reporter.SocializeEvent -= OnConversationEnded;
        Managers.Reporter.SocializeEvent += OnConversationEnded;
    }

    public void OnConversationEnded(string npcName)
    {
        if (IsContacted(npcName) || IsComplete())
            return;

        _contactedNPC.Add(npcName);
        ReceiveReport(1);

        if (CurrentSuccess >= SuccessToComplete)
        {
            ObjectiveState = Define.ObjectiveState.Complete;
        }

    }

    bool IsContacted(string npcName)
    {
        foreach (var npc in _contactedNPC)
        {
            if (npcName.Contains(npc))
                return true;
        }

        return false;
    }
}
