using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectiveGroup : MonoBehaviour
{
    List<Objective> _objectivList;
    public Quest Owner { get; private set; }
    ObjectiveGroupState _objectiveGroupState;
    public ObjectiveGroupState ObjectiveGroupState { get; private set; }


    public bool IsComplete()
    {
        return ObjectiveGroupState == ObjectiveGroupState.Complete;
    }
    public bool IsAllObjectiveComplete()
    {
        foreach(Objective obj in _objectivList)
        {
            if(!obj.IsComplete())
            {
                return false;
            }
        }

        return true;
    }
}
