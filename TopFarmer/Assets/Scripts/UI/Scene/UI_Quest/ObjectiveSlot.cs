using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveSlot : MonoBehaviour
{
    [SerializeField] Text _objectiveDescriptionText;
    [SerializeField] Text _objectiveProgressText;
   
    public void SetObjectiveSlot(Objective objective)
    {
        _objectiveDescriptionText.text = objective.ObjectiveDescription;
        _objectiveProgressText.text = string.Format("{0}/{1}", objective.CurrentSuccess, objective.SuccessToComplete);
    }
}
