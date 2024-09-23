using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestQuest : Quest
{
    public int TargetItemId { get; private set; }
    public int TargetQuantity { get; private set; }

    public HarvestQuest(int questId)
    {
        Init(questId);
    }

    void Init(int questId)
    {

        QuestData questData = null;
        Managers.Data.QuestDict.TryGetValue(questId, out questData);
        if (questData == null)
            return;

        HarvestQuestData data = (HarvestQuestData)questData;

        TargetItemId = data.targetItemId;
        TargetQuantity = data.targetQuantity;
    }
}
