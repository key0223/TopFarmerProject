using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestDetail : MonoBehaviour
{
    [SerializeField] Text _questTitleText;
    [SerializeField] Text _questDescriptionText;
    [SerializeField] GameObject _objectiveParent;

    [SerializeField] Button _backButton;
    [SerializeField] GameObject _questListGO;
    
    GameObject _questDetail;
    UI_QuestList _questListUI;

    Quest _quest;

    private void Awake()
    {
        _backButton.onClick.AddListener(()=> OnBackButtonClicked());
        _questListUI = gameObject.GetComponentInParent<UI_QuestList>();
    }
    public void SetQuestDetail(Quest quest)
    {
      _quest = quest;
        _questTitleText.text = quest.QuestTitle;
        _questDescriptionText.text = quest.QuestDescription;

        ClearObjectives();

        GameObject objectiveSlotGO = Managers.Resource.Instantiate("UI/Scene/Quest/ObjectiveSlot", _objectiveParent.transform);
        ObjectiveSlot objectiveSlot = objectiveSlotGO.GetComponent<ObjectiveSlot>();
        objectiveSlot.SetObjectiveSlot(quest.Objective);
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

    void OnBackButtonClicked()
    {
        _questListGO.SetActive(true);
        _questListUI.ReloadeQuestList();
        gameObject.SetActive(false);
    }
}
