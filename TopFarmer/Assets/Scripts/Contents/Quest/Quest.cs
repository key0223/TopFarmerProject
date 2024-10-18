
using UnityEngine;
using static Define;

public class Quest 
{
    public delegate void CompletedHandler(Quest quest);
    public event CompletedHandler onCompleted;

    public int QuestId { get; protected set; }
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

        int testMakeQuestIndex = 3;

        switch ((QuestType)testMakeQuestIndex)
        {
            case QuestType.Basic:
                quest = new Quest();
                break;
            case QuestType.ItemDelivery:
                quest = new ItemDeliveryQuest();
                break;
            case QuestType.Monster:
                quest = new MonsterQuest();
                break;
            case QuestType.Socialize:
                quest = new SocializeQuest();
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

    public void Complete()
    {
        Objective.Complete();
        QuestState = QuestState.Complete;

        GiveReward();
        onCompleted?.Invoke(this);
        onCompleted = null;

    }

    void GiveReward()
    {
        PlayerController.Instance.PlayerCoin += MoneyReward;
    }
    
    public virtual QuestSave SaveQuest()
    {
        QuestSave questSave = new QuestSave()
        {
            questId = QuestId,
            questType = QuestType,
            questTitle = QuestTitle,
            questDescription = QuestDescription,
            questObjective = QuestObjective,
            nextQuest = NextQuest,
            itemReward = ItemReward,
            moneyReward = MoneyReward,
            cancellable = Cancellable,
            reactionText = ReactionText,
            dailyQuest = DailyQuest,
            questState = QuestState,
            objective = Objective.SaveObjective(),

        };
        return questSave;
    }

    public static Quest LoadQuest(QuestSave questSave)
    {
        Quest quest = null;

        if(questSave.dailyQuest)
        {
            quest = Quest.MakeQuest();
        }
        else
        {
            quest = MakeQuest(questSave.questId);

        }

        if (quest == null)
            return null;

        // QuestSave의 데이터를 덮어씌움
        quest.QuestTitle = questSave.questTitle;
        quest.QuestType = questSave.questType;
        quest.QuestDescription = questSave.questDescription;
        quest.QuestObjective = questSave.questObjective;
        quest.NextQuest = questSave.nextQuest;
        quest.ItemReward = questSave.itemReward;
        quest.MoneyReward = questSave.moneyReward;
        quest.Cancellable = questSave.cancellable;
        quest.ReactionText = questSave.reactionText;
        quest.DailyQuest = questSave.dailyQuest;
        quest.QuestState = questSave.questState;

        // Objective 로드
        if (questSave.objective != null)
        {
            quest.Objective = Objective.LoadObjective(questSave.objective, quest);
        }

       if(quest is MonsterQuest monsterQuest)
        {
            MonsterQuestSave monsterQuestSave = JsonUtility.FromJson<MonsterQuestSave>(questSave.subClassData);
            monsterQuest.TargetName = monsterQuestSave.targetName;
            monsterQuest.TargetMonsterId = monsterQuestSave.targetMonsterId;
            monsterQuest.TargetMonsterName = monsterQuestSave.targetMonsterName;
            monsterQuest.TargetQuantity = monsterQuestSave.targetQuantity;
        }

        return quest;


    }
}
