using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterQuest : Quest
{
    public string TargetName { get; private set; }
    public int TargetQuantity { get; private set; }

    string _questStringId = "SlayMonsterQuest";
    public MonsterQuest(int questId)
    {
        Init(questId);
    }

    void Init(int questId)
    {

        QuestData questData = null;
        Managers.Data.QuestDict.TryGetValue(questId, out questData);
        if (questData == null)
            return;

        MonsterQuestData data = (MonsterQuestData)questData;

        TargetName = data.targetName;
        TargetQuantity = data.targetQuantity;
    }

    public MonsterQuest()
    {
        QuestType = Define.QuestType.Monster;
        QuestTitle = Managers.Data.StringDict[_questStringId + 1.ToString()].ko;

    }
}
