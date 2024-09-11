

using UnityEngine;

public class ItemDeliveryQuest : Quest
{
    public string TargetName { get; private set; }
    public int TargetItemId { get; private set; }
    public int TargetQuantity { get; private set; }

    string _questStringId = "ItemDeliveryQuest";
    public ItemDeliveryQuest(int questId)
    {
        Init(questId);
    }

    void Init(int questId)
    {

        QuestData questData = null;
        Managers.Data.QuestDict.TryGetValue(questId, out questData);
        if (questData == null)
            return;

        ItemDeliveryQuestData data = (ItemDeliveryQuestData)questData;

        TargetName = data.targetName;
        TargetItemId = data.targetItemId;
        TargetQuantity = data.targetQuantity;
    }

    public ItemDeliveryQuest()
    {
        // Make random quest
        QuestType = Define.QuestType.ItemDelivery;
        QuestTitle = Managers.Data.StringDict["ItemDeliveryQuest1"].ko;

        TargetName = "Abigail";
        int randDescriptionIndex = Random.Range(0, 4);

        switch (randDescriptionIndex)
        {
            case 0:
                {
                    TargetItemId = Random.Range(301, 309);
                    TargetQuantity = 1;
                    string itemName = Managers.Data.StringDict[$"itemName({TargetItemId.ToString()})"].ko;

                    QuestDescription = string.Format(CombineQuestDescription(randDescriptionIndex), itemName);
                }
                break;
            case 1:
                {
                    TargetItemId = 324;
                    TargetQuantity = Random.Range(1, 11);
                    string itemName = Managers.Data.StringDict[$"itemName({TargetItemId.ToString()})"].ko;

                    QuestDescription = string.Format(CombineQuestDescription(randDescriptionIndex), itemName, TargetQuantity);
                }
                break;
            case 2:
                {
                    TargetItemId = Random.Range(301, 309);
                    TargetQuantity = 1;
                    string itemName = Managers.Data.StringDict[$"itemName({TargetItemId.ToString()})"].ko;

                    QuestDescription = string.Format(CombineQuestDescription(randDescriptionIndex), itemName);
                }
                break;
            case 3:
                {
                    TargetItemId = Random.Range(301, 309);
                    TargetQuantity = 1;
                    string itemName = Managers.Data.StringDict[$"itemName({TargetItemId.ToString()})"].ko;

                    QuestDescription = string.Format(CombineQuestDescription(randDescriptionIndex), itemName);
                }
                break;

        }

        QuestObjective = string.Format("{0}에게 {1} 배달하기.", TargetName, Managers.Data.StringDict[$"itemName({TargetItemId.ToString()})"].ko);
        NextQuest = -1;
        ItemReward = -1;
        MoneyReward = randDescriptionIndex == 3 ? Managers.Data.ShopItemDict[TargetItemId].sellPrice * 3 : Random.Range(150, 251);
        Cancellable = true;
        ReactionText = GetReactionText();

        Objective = new Objective(this);

    }

    string CombineQuestDescription(int descriptionIndex)
    {
        string description = "";
        switch (descriptionIndex)
        {
            case 0:
                {
                    // need crop itemId
                    int index1 = Random.Range(2, 5);
                    int index2 = Random.Range(5, 7);
                    int index3 = Random.Range(7, 10);

                    string str1 = Managers.Data.StringDict[_questStringId + index1.ToString()].ko;
                    string str2 = Managers.Data.StringDict[_questStringId + index2.ToString()].ko;
                    string str3 = Managers.Data.StringDict[_questStringId + index3.ToString()].ko;

                    description = str1 + str2 + str3;
                }
                break;
            case 1:
                {
                    description = "{0} {1}개가 필요합니다. 오늘 중으로 배달해주세요.";
                }
                break;
            case 2:
                {
                    // need food itemId
                    int index1 = Random.Range(14, 21);
                    int[] indexList = { 35, 38, 41, 42 };
                    int index2 = indexList[Random.Range(0, indexList.Length)];

                    string str1 = Managers.Data.StringDict[_questStringId + index1.ToString()].ko;
                    string str2 = Managers.Data.StringDict[_questStringId + index2.ToString()].ko;

                    description = str1 + str2;
                }
                break;
            case 3:
                {
                    // need itemId
                    int index1 = Random.Range(51, 52);
                    string str1 = Managers.Data.StringDict[_questStringId + index1.ToString()].ko;

                    description = str1;
                }
                break;
        }

        return description;
    }
    string GetReactionText()
    {
        int reactionIndex = Random.Range(53, 66);

        string reactionText = Managers.Data.StringDict[_questStringId + reactionIndex.ToString()].ko;

        return reactionText;
    }
}
