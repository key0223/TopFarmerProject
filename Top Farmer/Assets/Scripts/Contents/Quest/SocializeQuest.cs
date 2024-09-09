using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocializeQuest : Quest
{
    public string TargetName { get; private set; }

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

        TargetName = data.targetName;
    }
}
