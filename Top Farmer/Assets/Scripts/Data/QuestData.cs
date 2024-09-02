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
    public string questDeliverTimeRaw;
    public int questDeliverYear;
    public Season questDeliverSeason;
    public int questDeliverDay;
    public string questTitle;
    public string questDescription;
    public string questObjectiveRaw;
    public int[] questObjectiveIds;
    public int nextQuest;
    public int moneyReward;
    public string rewardDescription;
    public bool canBeCancelled;
    public string reactionText;
}


