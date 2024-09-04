using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestDetail : MonoBehaviour
{
    [SerializeField] Text _questTitleText;
    [SerializeField] Text _questDescriptionText;
    [SerializeField] GameObject _objectiveParent;

    [SerializeField] Button _backButton;

    Quest _quest;

    public void SetQuestDetail(Quest quest)
    {
        _quest = quest;
        _questTitleText.text = quest.QuestTitle;
        _questDescriptionText.text = quest.QuestDescription;

        ClearObjectives();
        if(quest.Objectives.Count > 0 )
        {
            foreach(Objective objective in quest.Objectives)
            {
                GameObject objectiveSlotGO = Managers.Resource.Instantiate("UI/Scene/Quest/ObjectiveSlot",_objectiveParent.transform);
                ObjectiveSlot objectiveSlot = objectiveSlotGO.GetComponent<ObjectiveSlot>();
                objectiveSlot.SetObjectiveSlot(objective);
            }
        }
    }
    
    void ClearObjectives()
    {
        for (int i = _objectiveParent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = _objectiveParent.transform.GetChild(i);

            ObjectiveSlot objectiveSlot = child.GetComponent<ObjectiveSlot>();

            if (objectiveSlot != null)
            {
               Managers.Resource.Destroy(objectiveSlot.gameObject);
            }
        }

    }
}
