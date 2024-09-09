using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterQuest : Quest
{
    public string TargetName { get; private set; }
    public int TargetQuantity { get; private set; }

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
}
