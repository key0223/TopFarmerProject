
using UnityEngine;
using static Define;

public class Quest 
{
    public int QuestId { get; private set; }
    public QuestType QuestType { get; protected set; }
    public string QuestTitle { get; protected set; }
    public string QuestDescription { get; protected set; }
    public string QuestObjective { get; protected set; }
    public int NextQuest {  get; protected set; }
    public int ItemReward { get; protected set; }
    public int MoneyReward { get; protected set; }
    public bool Cancellable { get; protected set; }
    public string ReactionText { get; protected set; }

    public bool DailyQuest { get; protected set; }
    public Objective Objective { get; protected set; }

    public QuestState QuestState { get; protected set; }
    public static Quest MakeQuest(int questId)
    {
        if (Managers.Data.QuestDict == null)
            return null;

        QuestData questData;
        Managers.Data.QuestDict.TryGetValue(questId, out questData);
        if (questData == null)
            return null;

        
        Quest quest = null;

        switch(questData.questType)
        {
            case QuestType.Basic:
                quest = new Quest();
                break;
            case QuestType.ItemDelivery:
                quest = new ItemDeliveryQuest(questId);
                break;
            case QuestType.Monster:
                quest = new MonsterQuest(questId);
                break;
            case QuestType.Socialize:
                quest = new SocializeQuest(questId);
                break;
            case QuestType.Location:
                quest = new LocationQuest(questId);
                break;
            case QuestType.Harvest:
                quest = new LocationQuest(questId);
                break;
        }

        if(quest !=null)
        {
            quest.QuestId = questData.questId;
            quest.QuestType = questData.questType;
            quest.QuestTitle = questData.questTitle;
            quest.QuestDescription = questData.questDescription;
            quest.QuestObjective = questData.questObjective;
            quest.NextQuest = questData.nextQuest;
            quest.ItemReward = questData.itemReward;
            quest.MoneyReward = questData.moneyReward;
            quest.Cancellable = questData.cancellable;
            quest.ReactionText = questData.reactionText;
            quest.DailyQuest = false;
        }

        return quest;
    }

    public static Quest MakeQuest()
    {
        int randomQuestType = Random.Range(2, 5);
        Quest quest = null;

        int testMakeQuestIndex = 2;

        switch ((QuestType)testMakeQuestIndex)
        {
            case QuestType.Basic:
                quest = new Quest();
                break;
            case QuestType.ItemDelivery:
                quest = new ItemDeliveryQuest();
                break;
            case QuestType.Monster:
                //quest = new MonsterQuest(questId);
                break;
            case QuestType.Socialize:
                //quest = new SocializeQuest(questId);
                break;
            case QuestType.Location:
                //quest = new LocationQuest(questId);
                break;
            case QuestType.Harvest:
                //quest = new LocationQuest(questId);
                break;
        }

        quest.DailyQuest = true;
        return quest;
    }

    public void OnRegister()
    {
        QuestState = QuestState.Running;
    }

    public void CheckObjectiveComplete(Objective objective, ObjectiveState currentState, ObjectiveState prevState)
    {
        if (Objective.ObjectiveState == ObjectiveState.Complete)
        {
            QuestState = QuestState.WatingForCompletion;
        }
    }
    
}
