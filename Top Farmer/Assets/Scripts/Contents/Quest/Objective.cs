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
    public Quest Owner { get; private set; } 
    public QuestType QuestType { get; private set; }
    public string ObjectiveTitle {  get; private set; } 
    public string ObjectiveDescription { get; private set; }
    public int SuccessToComplete { get; private set; }


    public List<ObjectiveTarget> ObjectiveTargets { get; private set; }
    IObjectiveAction _objectiveAction;

    ObjectiveState _objectiveState;
    int _currentSuccess;

    public Objective(int objectiveId)
    {
        ObjectiveData objectiveData = null;
        if(Managers.Data.ObjectiveDict.TryGetValue(objectiveId, out objectiveData))
        {
            QuestType = objectiveData.questType;
            ObjectiveDescription = objectiveData.objectiveDescription;
            SuccessToComplete = objectiveData.needSuccessToComplete;
            
            switch(objectiveData.questType)
            {
                case QuestType.Basic:
                    foreach(string target in objectiveData.targets)
                    {
                        ObjectiveTargets.Add(new StringTarget(target));
                    }
                    _objectiveAction = new SimpleCountAction();
                    break;
               
                case QuestType.ItemDelivery:
                    foreach (string target in objectiveData.targets)
                    {
                        ObjectiveTargets.Add(new StringTarget(target));
                    }
                    _objectiveAction = new SimpleCountAction();
                    break;
                case QuestType.Monster:
                    foreach (string target in objectiveData.targets)
                    {
                        ObjectiveTargets.Add(new StringTarget(target));
                    }
                    _objectiveAction = new SimpleCountAction();
                    break;
                case QuestType.Socialize:
                    foreach (string target in objectiveData.targets)
                    {
                        ObjectiveTargets.Add(new StringTarget(target));
                    }
                    _objectiveAction = new SimpleCountAction();
                    break;
                case QuestType.Harvest:
                    foreach (string target in objectiveData.targets)
                    {
                        ObjectiveTargets.Add(new StringTarget(target));
                    }
                    _objectiveAction = new SimpleCountAction();
                    break;
                case QuestType.Resource:
                    foreach (string target in objectiveData.targets)
                    {
                        ObjectiveTargets.Add(new StringTarget(target));
                    }
                    _objectiveAction = new SimpleCountAction();
                    break;
            }
        }
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

    public void Setup(Quest owner)
    {
        Owner = owner;
    }
    public void Start()
    {
        ObjectiveState = ObjectiveState.Running;

    }
    public void End() //Objective 완료 시 이벤트 리셋
    {
        OnStateChanged = null;
        OnSuccessChanged = null;
    }

    public bool IsComplete()
    {
        return ObjectiveState == ObjectiveState.Complete;
    }

  
    
    public void ReceiveReport(int successCount)
    {
        CurrentSuccess = _objectiveAction.Run(this,CurrentSuccess,successCount);
    }

   
    
    public void Complete() // Objective 즉시 완료
    {
        CurrentSuccess = SuccessToComplete;
    }

    public bool IsTarget(QuestType questType, object target)
    {
        if (QuestType != questType)
            return false;

        bool targetFound = false;

        foreach (var objectiveTarget in ObjectiveTargets)
        {
            if (objectiveTarget.IsEquals(target))
            {
                targetFound = true;
                break;

            }
        }

        if(!targetFound) return false;
        if (!IsComplete()) return true;

        return false;
    }
}
