using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestList : MonoBehaviour
{
    [SerializeField] Button _closeButton;
    [SerializeField] Button _questListButton;
    [SerializeField] GameObject _questListPanel;

    [Header("Content")]
    [SerializeField] GameObject _content;
    private void Awake()
    {
        _closeButton.onClick.AddListener(() => OnCloseButtonClicked());
        _questListButton.onClick.AddListener(() => OnQuestListButtonClicked());
    }

    void OnQuestListButtonClicked()
    {
        _questListPanel.SetActive(true);
        ClearQuestList();
        InitalizeQuestList();
    }
    void OnCloseButtonClicked()
    {
        _questListPanel.SetActive(false);
    }

    void InitalizeQuestList()
    {
        foreach(Quest quest in Managers.Quest.ActiveQuests)
        {
            GameObject questSlotGO = Managers.Resource.Instantiate("UI/Scene/Quest/QuestSlot", _content.transform);
            UI_QuestSlot questSlot = questSlotGO.GetComponent<UI_QuestSlot>();
            questSlot.SetQuestSlot(quest);
        }
      
    }

    void ClearQuestList()
    {
        if(_content.transform.childCount>0)
        {
            for (int i = _content.transform.childCount -1; i >=0; i--)
            {
                Transform child = _content.transform.GetChild(i);
                Managers.Resource.Destroy(child.gameObject);
            }
        }
    }
}
