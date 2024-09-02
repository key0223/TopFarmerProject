using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Define;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.Progress;

public class Quest 
{
    #region Events

    public delegate void ObjectiveSuccessChangedHandler(Quest quest, Objective objective, int currentSuccess, int prevSuccess);
    public delegate void CompletedHandler(Quest quest);
    public delegate void CanceledHandler(Quest quest);

    #endregion

    public event ObjectiveSuccessChangedHandler onObjectiveSuccessChanged;
    public event CompletedHandler onCompleted;
    public event CanceledHandler onCanceled;
    /*
      Quest Id /Quest Type /Quest Title /Quest Description /퀘스트 목표 /Quest Target& Item Id /다음 퀘스트 여부 / 취소가능한 퀘스트? /완료 시 대사
    */
    public int QuestId { get; private set; }
    public string QuestTitle { get; private set; }
    public string QuestDescription { get; private set; }

    public string RewardDescription { get; private set; }
    public string CompletionString { get; private set; }


    public bool CanBeCancelled { get; private set; }

    public int MoneyReward { get; private set; }
    public QuestType QuestType { get; private set; }

    public List<Objective> Objectives { get; private set; }
    public int NextQuest { get; private set; }

    public bool _accepted;
    public bool _dailyQuest;
    public string _currentObjective;
    public int _daysLeft;
    public bool _destroy;


    bool _loadedTitle;
    bool _loadedDescription;

    public QuestState State { get; private set; }
    public bool IsComplete => State == QuestState.Complete;
    public bool IsCompletable => State == QuestState.WatingForCompletion;
    public bool IsAllObjectiveComplete()
    {
        bool allCompleted = true;
        foreach(Objective objective in Objectives)
        {
            if(!objective.IsComplete())
            {
                allCompleted = false;
                break;
            }
        }

        return allCompleted;
    }
    public Quest(int questId )
    {
        QuestData questData = null;
        if(Managers.Data.QuestDict.TryGetValue( questId, out questData ) )
        {
            QuestId = questData.questId;
            QuestTitle = questData.questTitle;
            QuestDescription = questData.questDescription;
            RewardDescription = questData.rewardDescription == "-1" ? "" : questData.rewardDescription;
            CompletionString = questData.reactionText;
            CanBeCancelled = questData.canBeCancelled;
            MoneyReward = questData.moneyReward;
            QuestType = questData.questType;
            NextQuest = questData.nextQuest;
        }
       
    }
    public void AddObjective(Objective objective)
    {
        Objectives.Add( objective );
    }
   
    public string CurrentObjective
    {
        get
        {
            QuestData questData = null;

            if (Managers.Data.QuestDict.TryGetValue(QuestId, out questData))
            {
               // _currentObjective = questData.questObjective;
            }
            reloadObjective();

            if (_currentObjective == null)
            {
                _currentObjective = "";
            }
            return _currentObjective;
        }

        set { _currentObjective = value; }
    }
   
    public void OnRegister()
    {
        foreach(Objective objective in Objectives)
        {
            objective.Setup(this);
            objective.OnSuccessChanged += OnSuccessChanged;
            objective.Start();
        }
    }
    public void ReceiveReport(QuestType questType, object target, int successCount )
    {
        if (IsComplete)
            return;

        foreach(Objective objective in Objectives)
        {
            if (objective.IsTarget(questType, target))
                objective.ReceiveReport(successCount);
        }

        if(IsAllObjectiveComplete())
        {
            State = QuestState.WatingForCompletion;
        }
        else
        {
            State = QuestState.Running;
        }

    }

    public void Complete()
    {
        if (IsComplete)
            return;
        State = QuestState.Complete;

        foreach(Objective objective in Objectives)
        {
            if(!objective.IsComplete())
            {
                objective.Complete();
            }
        }

        onCompleted?.Invoke(this);

        onObjectiveSuccessChanged = null;
        onCompleted = null;
        onCanceled = null;
    }

    public void Cancel()
    {
        State = QuestState.Cancel;
        onCanceled?.Invoke(this);
    }
    public virtual void reloadObjective()
    {
    }
    public virtual void reloadDescription()
    {
    }

    public virtual void adjustGameLocation(Scene location)
    {
    }
    public virtual bool CheckIfCompleted() { return true; }
    public bool HasReward()
    {
        if (HasMoneyReward())
        {
            return true;
        }
        return RewardDescription != null && RewardDescription.Length > 2;
    }
    public virtual bool IsSecretQuest() => false;
   
    public bool IsHidden() => IsSecretQuest();
    public List<string> GetObjectiveDescription() => new List<string>() { _currentObjective };


    public bool HasMoneyReward() => IsComplete && MoneyReward > 0;

    public bool ShouldDisplayAsComplete() => IsComplete && !IsHidden();
    public bool IsTimedQuest() => _dailyQuest;
    public int GetDaysLeft() => _daysLeft;
    public int GetMoneyReward() => MoneyReward;

    // 금전적 보상을 수령했을 때 퀘스트가 삭제될 수 있도록 설정
    public void OnMoneyRewardClaimed()
    {
        MoneyReward = 0;
        _destroy = true;
    }

    public bool OnLeaveQuestPage()
    {
        if (IsComplete && MoneyReward <= 0)
            _destroy = true;
        if (!_destroy)
            return false;

        // TODO : 퀘스트 삭제

        return true;
    }

    void OnSuccessChanged(Objective objective, int currentSuccess, int prevSuccess)
        => onObjectiveSuccessChanged?.Invoke(this, objective, currentSuccess, prevSuccess);
}
