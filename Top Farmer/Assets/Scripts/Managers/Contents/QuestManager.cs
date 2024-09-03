using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class QuestManager : MonoBehaviour
{
    #region Events
    public delegate void QuestRegisteredHandler(Quest newQuest);
    public delegate void QuestCompletedHandler(Quest quest);
    public delegate void QuestCanceledHandler(Quest quest);
    #endregion

    public event QuestRegisteredHandler onQuestRegistered;
    public event QuestCompletedHandler onQuestCompleted;
    public event QuestCanceledHandler onQuestCanceled;

    List<Quest> _activeQuests = new List<Quest>();
    List<Quest> _completedQuests = new List<Quest>();
    public List<Quest> ActiveQuests { get { return _activeQuests; } }
    public Quest CreateQuest(int questId)
    {
        Quest quest = new Quest(questId);

        QuestData questData = null;
        if (Managers.Data.QuestDict.TryGetValue(questId, out questData))
        {
            foreach (int objectiveId in questData.questObjectiveIds)
            {
                Objective objective = new Objective(objectiveId);
                quest.AddObjective(objective);
            }

        }
        return quest;
    }

    public void Register(Quest quest)
    {
        quest.onCompleted += OnQuestCompleted;
        quest.onCanceled += OnQuestCanceled;

        _activeQuests.Add(quest);
        quest.OnRegister();
        onQuestRegistered?.Invoke(quest);

        Debug.Log(ActiveQuests.Count.ToString());
    }

    public void ReceiveReport(QuestType questType, object target, int successCount)
    {
        ReceiveReport(_activeQuests, questType, target, successCount);

    }
    public void ReceiveReport(QuestType questType, ObjectiveTarget target, int successCount)
    {
        ReceiveReport(questType, target.Value, successCount);
    }

    private void ReceiveReport(List<Quest> quests, QuestType questType, object target, int successCount)
    {
        foreach (var quest in quests.ToArray())
            quest.ReceiveReport(questType, target, successCount);
    }
    public void CompleteWaitingQuests()
    {
        foreach (var quest in _activeQuests.ToList())
        {
            if (quest.IsCompletable)
                quest.Complete();
        }
    }
    public bool ContainsActiveQuests(QuestData quest) => _activeQuests.Any(x => x.QuestId == quest.questId);
    public bool ContainsCompleteQuests(QuestData quest)=> _completedQuests.Any(x=>x.QuestId == quest.questId);
    
    public List<QuestData> GetAcceptableQuestData()
    {
        List<QuestData> foundQuests = new List<QuestData>();
        foreach (QuestData questData in Managers.Data.QuestDict.Values)
        {
            if (CanAcceptQuest(questData) && !ContainsActiveQuests(questData) && !ContainsActiveQuests(questData))
            {
                foundQuests.Add(questData);
            }
        }

        return foundQuests;
    }

    bool CanAcceptQuest(QuestData questData)
    {
        if(!IsAvailableDate(questData))
            return false;

        return true;
    }
   bool IsAvailableDate(QuestData questData)
    {
        if (questData ==null|| questData.questDeliverTimeRaw==null)
            return false;

        string gameYearStr = TimeManager.Instance.GameYear.ToString();
        string seasonStr = "";
        switch(TimeManager.Instance.GameSeason)
        {
            case Define.Season.SPRING:
                seasonStr = "SPRING";
                break;
            case Define.Season.SUMMER:
                seasonStr = "SUMMER";
                break;
            case Define.Season.AUTUMN:
                seasonStr = "AUTUMN";
                break;
            case Define.Season.WINTER:
                seasonStr = "WINTER";
                break;
        }
        string gameDay = TimeManager.Instance.GameDay.ToString();
        string currentDate = $"{gameYearStr},{seasonStr},{gameDay}";
        string questDate = questData.questDeliverTimeRaw.Replace(" ", "");

        return currentDate.Equals(questDate);

    }

    //Qeust event에 등록하는 Callback 함수
    #region Callback
    private void OnQuestCompleted(Quest quest)
    {
        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);

        onQuestCompleted?.Invoke(quest);

    }

    private void OnQuestCanceled(Quest quest)
    {
        _activeQuests.Remove(quest);
        onQuestCanceled?.Invoke(quest);

        //Destroy(quest, Time.deltaTime);
    }
    #endregion
}
