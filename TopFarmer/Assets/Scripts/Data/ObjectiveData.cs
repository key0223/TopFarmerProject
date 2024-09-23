using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[Serializable]
public class ObjectiveData 
{
    public int ownerQuestId;
    public int objectiveId;
    public QuestType questType;
    public string objectiveTitle;
    public string objectiveDescription;

    public ObjectiveActionType actionType;

    public string targetsRaw;
    public string[] targets;

    public int needSuccessToComplete;

}
