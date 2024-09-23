using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocializeQuest : Quest
{
    public string[] TargetNames { get; private set; }
    public int TargetCount { get; private set; }

    string _questStringId = "SocializeQuest";
    
    public SocializeQuest(int questId)
    {
        Init(questId);
    }

    void Init(int questId)
    {

        QuestData questData = null;
        Managers.Data.QuestDict.TryGetValue(questId, out questData);
        if (questData == null)
            return;

        SocializeQuestData data = (SocializeQuestData)questData;

        //TargetName = data.targetName;
    }

    public SocializeQuest()
    {
        QuestType = Define.QuestType.Socialize;
        QuestTitle = Managers.Data.StringDict[$"{_questStringId}1"].ko;
        TargetNames = new string[] { "Abigail", "Clint" };
        TargetCount = Random.Range(1, TargetNames.Length + 1);


        QuestDescription = SetQuestDescription();
        QuestObjective = string.Format("마을주민 {0}명에게 인사하기.", TargetCount);
        NextQuest = -1;
        ItemReward = -1;
        MoneyReward = 100;
        Cancellable = true;
        ReactionText = null;


        Objective = Objective.MakeObjective(this);
        Objective.OnStateChanged += CheckObjectiveComplete;
    }

    string SetQuestDescription()
    {
        int randDescriptionIndex = Random.Range(3, 6);

        string baseDescription = Managers.Data.StringDict[_questStringId + 2.ToString()].ko;
        string randDescription = Managers.Data.StringDict[_questStringId + randDescriptionIndex.ToString()].ko;

        string resultDescriptopm = string.Format(baseDescription, randDescription);

        return resultDescriptopm;
    }

  
}
