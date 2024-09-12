using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Objective 
{
    #region Event
    public delegate void StateChangedHandler(Objective objective, ObjectiveState currentState, ObjectiveState prevState);
    public delegate void SuccessChangedHandler(Objective objective, int curentSucess, int prevSucess); //currentSuccess값이 변했을 때 알려주는 event

    public event StateChangedHandler OnStateChanged;
    public event SuccessChangedHandler OnSuccessChanged;

    #endregion
    public Quest Owner { get; protected set; } 
    public ObjectiveType ObjectiveType { get; protected set; }
    public string ObjectiveDescription { get; protected set; }
    public int SuccessToComplete { get; protected set; }


    protected IObjectiveAction _objectiveAction;
    protected ObjectiveState _objectiveState;
    int _currentSuccess;


    public static  Objective MakeObjective(Quest quest)
    {
        Objective objective = null;
        switch (quest.QuestType)
        {
            case QuestType.ItemDelivery:

                objective = new DeliverObjective(quest);

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

        objective.Owner = quest;
        objective.ObjectiveDescription = quest.QuestObjective;

        return objective;
    }
   
   
    public int CurrentSuccess
    {
        get { return _currentSuccess; }
        set
        {
            int prevSuccess = _currentSuccess;
            _currentSuccess = Mathf.Clamp(value, 0,SuccessToComplete );
            if(_currentSuccess != prevSuccess)
            {
                _objectiveState = _currentSuccess == SuccessToComplete ? ObjectiveState.Complete : ObjectiveState.Running;
                OnSuccessChanged?.Invoke(this,_currentSuccess, prevSuccess); // 바뀐 값 보고

            }
        }
    }

    public ObjectiveState ObjectiveState
    {
        get { return _objectiveState; }
        set
        {
            ObjectiveState prevState = _objectiveState;
            _objectiveState = value;
            OnStateChanged?.Invoke(this,_objectiveState, prevState);
        }
    }

    public virtual void Start()
    {
        ObjectiveState = ObjectiveState.Running;

    }
    public void End() //Objective 완료 시 이벤트 리셋
    {
        OnStateChanged = null;
        OnSuccessChanged = null;
    }
    public void Complete()
    {
        CurrentSuccess = SuccessToComplete;
    }

    public bool IsComplete()
    {
        return ObjectiveState == ObjectiveState.Complete;
    }
    
    public void ReceiveReport(int successCount)
    {
        CurrentSuccess = _objectiveAction.Run(this,CurrentSuccess,successCount);
    }

    

    public bool IsTarget(ObjectiveType objectiveType, object target)
    {
        if (ObjectiveType != objectiveType)
            return false;

        bool targetFound = false;

        //foreach (var objectiveTarget in ObjectiveTargets)
        //{
        //    if (objectiveTarget.IsEquals(target))
        //    {
        //        targetFound = true;
        //        break;

        //    }
        //}

        if(!targetFound) return false;
        if (!IsComplete()) return true;

        return false;
    }
}
