using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[Serializable]
public class QuestSave
{
    public int questId;
    public QuestType questType;
    public string questTitle;
    public string questDescription;
    public string questObjective;
    public int nextQuest;
    public int itemReward;
    public int moneyReward;
    public bool cancellable;
    public string reactionText;
    public bool dailyQuest;
    public QuestState questState;
    public ObjectiveSave objective;

    public string subClassData;
    
}

[Serializable]
public class ObjectiveSave
{
    public int ownerQuestId;
    public ObjectiveType objectiveType;
    public string objectiveDescription;
    public int successToComplete;
    public int currentSuccess;
    public ObjectiveState objectiveState;
}

[Serializable]
public class MonsterQuestSave
{
    public string targetName;
    public int targetMonsterId;
    public string targetMonsterName;
    public int targetQuantity;
}



