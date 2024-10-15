using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[Serializable]
public class QuestData
{
    public int questId;
    public QuestType questType;
    public int questDeliverDay;
    public string questTitle;
    public string questDescription;
    public string questObjective;
    public int nextQuest;
    public int itemReward;
    public int moneyReward;
    public bool cancellable;
    public string reactionText;
}

public class ItemDeliveryQuestData : QuestData
{
    public string target;
    public string targetName;
    public int targetItemId;
    public int targetQuantity;
}
public class MonsterQuestData : QuestData
{
    public string target;
    public string targetName;
    public int targetMonsterId;
    public int targetQuantity;
}
public class SocializeQuestData : QuestData
{
    public string target;
    public string targetName;
}
public class LocationQuestData : QuestData
{
    public string target;
    public string targetPlace;
}

public class HarvestQuestData:QuestData
{
    public string target;
    public int targetItemId;
    public int targetQuantity;
}


