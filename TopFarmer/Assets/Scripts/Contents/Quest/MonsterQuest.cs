using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterQuest : Quest
{
    public string TargetName { get;  set; } // TargetName indicates owner of the quest
    public int TargetMonsterId { get;  set; }
    public string TargetMonsterName { get;  set; }
    public int TargetQuantity { get;  set; }

    string _questStringId = "SlayMonsterQuest";
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

        QuestType = QuestType.Monster;
        QuestTitle = data.questTitle;
        TargetName = data.targetName;
        TargetMonsterId = data.targetMonsterId;
        TargetQuantity = data.targetQuantity;
        TargetMonsterName = Managers.Data.StringDict[$"monsterName({TargetMonsterId})"].ko;

        QuestDescription = data.questDescription;
        QuestObjective = string.Format("{0} {1}마리 처치하기.", TargetMonsterName, TargetQuantity);
        NextQuest = data.nextQuest;
        ItemReward = data.itemReward;
        MoneyReward = data.moneyReward;
        Cancellable = data.cancellable;
        ReactionText = data.reactionText;

        Objective = Objective.MakeObjective(this);
        Objective.OnStateChanged += CheckObjectiveComplete;

    }

    public MonsterQuest()
    {
        QuestType = QuestType.Monster;
        QuestTitle = Managers.Data.StringDict[_questStringId + 1.ToString()].ko;
        TargetName = "Abigail";
        
        int randDescriptionIndex = Random.Range(0, 3);

        // Make description depend on monsterType
        switch (randDescriptionIndex)
        {
            case 0:
                {
                    TargetMonsterId = 801; // Green Slime
                    TargetQuantity = Random.Range(5,11);
                    TargetMonsterName = Managers.Data.StringDict[$"monsterName({TargetMonsterId})"].ko;

                    QuestDescription = string.Format(CombineQuestDescription(randDescriptionIndex), TargetQuantity, TargetMonsterName);
                }
                break;
            case 1:
                {
                    TargetMonsterId = Random.Range(801, 804);
                    TargetQuantity = Random.Range(1, 11);
                    TargetMonsterName = Managers.Data.StringDict[$"monsterName({TargetMonsterId})"].ko;

                    QuestDescription = string.Format(CombineQuestDescription(randDescriptionIndex),TargetQuantity, TargetMonsterName);
                }
                break;
            case 2:
                {
                    TargetMonsterId = Random.Range(801, 804);
                    TargetQuantity = Random.Range(1, 11);
                    TargetMonsterName = Managers.Data.StringDict[$"monsterName({TargetMonsterId})"].ko;

                    QuestDescription = CombineQuestDescription(randDescriptionIndex);
                }
                break;
        }

        QuestObjective = string.Format("{0} {1}마리 처치하기.", TargetMonsterName, TargetQuantity);
        NextQuest = -1;
        ItemReward = -1;
        MoneyReward = Random.Range(300,1301);
        Cancellable = true;
        DailyQuest = true;
        ReactionText = GetReactionText(TargetMonsterId);

        Objective = Objective.MakeObjective(this);
        Objective.OnStateChanged += CheckObjectiveComplete;

    }

    string CombineQuestDescription(int descriptionIndex)
    {
        string description = "";

        switch (descriptionIndex)
        {
            case 0:
                {
                    description = "구함: {1} {0}마리 처치할 슬라임 헌터";
                }
                break;
            case 1:
                {
                    description = "구함: 지역 광산에서 {1} {0}마리를 처치할 몬스터 헌터 구함.";
                }
                break;
            case 2:
                {
                   string mainDec = Managers.Data.StringDict[_questStringId +16.ToString()].ko;
                    int adjIndex = Random.Range(17, 20);
                    string adjString = Managers.Data.StringDict[_questStringId + adjIndex.ToString()].ko;
                    description = string.Format(mainDec, TargetMonsterName, TargetQuantity,adjString) ;
                }
                break;
        }

        return description;
    }

    string GetReactionText(int monsterId)
    {
        if (monsterId <= 0)
            return null;

        string reactionText = "";
        switch (monsterId)
        {
            case 801:
                {
                    // Slime
                    int[] reactionIndex = { 4, 11 };
                    int randomIndex = Random.Range(0,reactionIndex.Length);
                    reactionText = Managers.Data.StringDict[_questStringId+randomIndex.ToString()].ko;
                }
                break;
            default:
                {
                    int randomIndex = Random.Range(63, 66);
                    reactionText = Managers.Data.StringDict["ItemDeliveryQuest" + randomIndex.ToString()].ko;
                }
                break;
        }

        return reactionText;
    }
    
    public override QuestSave SaveQuest()
    {
        QuestSave questSave = base.SaveQuest();

        MonsterQuestSave monsterQuestSave = new MonsterQuestSave()
        {
            targetName = TargetName,
            targetMonsterId = TargetMonsterId,
            targetMonsterName = TargetMonsterName,
            targetQuantity = TargetQuantity,
        };

        questSave.subClassData = JsonUtility.ToJson(monsterQuestSave);

        return questSave;
    }

    public static MonsterQuest LoadQuest(QuestSave questSave)
    {
        MonsterQuest monsterQuest = new MonsterQuest();

        if (questSave.dailyQuest)
        {
            monsterQuest = (MonsterQuest)MakeQuest();
        }
        else
        {
            monsterQuest = (MonsterQuest)MakeQuest(questSave.questId);

        }

        if (monsterQuest == null)
            return null;

        // QuestSave의 데이터를 덮어씌움
        monsterQuest.QuestTitle = questSave.questTitle;
        monsterQuest.QuestDescription = questSave.questDescription;
        monsterQuest.QuestObjective = questSave.questObjective;
        monsterQuest.NextQuest = questSave.nextQuest;
        monsterQuest.ItemReward = questSave.itemReward;
        monsterQuest.MoneyReward = questSave.moneyReward;
        monsterQuest.Cancellable = questSave.cancellable;
        monsterQuest.ReactionText = questSave.reactionText;
        monsterQuest.DailyQuest = questSave.dailyQuest;
        monsterQuest.QuestState = questSave.questState;

        // Objective 로드
        if (questSave.objective != null)
        {
            monsterQuest.Objective = Objective.LoadObjective(questSave.objective, monsterQuest);
        }

        MonsterQuestSave monsterQuestSave = JsonUtility.FromJson<MonsterQuestSave>(questSave.subClassData);
        monsterQuest.TargetName = monsterQuestSave.targetName;
        monsterQuest.TargetMonsterId = monsterQuestSave.targetMonsterId;
        monsterQuest.TargetMonsterName = monsterQuestSave.targetMonsterName;
        monsterQuest.TargetQuantity = monsterQuestSave.targetQuantity;

        return monsterQuest;


    }


}
