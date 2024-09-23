using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationQuest : Quest
{
    public string TargetPlace { get; private set; }

    public LocationQuest(int questId)
    {
        Init(questId);
    }

    void Init(int questId)
    {

        QuestData questData = null;
        Managers.Data.QuestDict.TryGetValue(questId, out questData);
        if (questData == null)
            return;

        LocationQuestData data = (LocationQuestData)questData;

        TargetPlace = data.targetPlace;
    }
}
