using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestList : MonoBehaviour
{
    [SerializeField] Button _closeButton;
    [SerializeField] Button _questListButton;
    [SerializeField] GameObject _questListPanel;
    [SerializeField] GameObject _questList;

    [Header("Content")]
    [SerializeField] GameObject _content;

    UI_QuestDetail _questDetail;

    private void Awake()
    {
        _closeButton.onClick.AddListener(() => OnCloseButtonClicked());
        _questListButton.onClick.AddListener(() => OnQuestListButtonClicked());
        _questDetail = GetComponentInChildren<UI_QuestDetail>();

        _questListPanel.SetActive(false);
        _questDetail.gameObject.SetActive(false);
    }
   
    void OnQuestListButtonClicked()
    {
        DisablePlayerInput();
        _questListPanel.SetActive(true);
        _questList.SetActive(true);
        ClearQuestList();
        InitalizeQuestList();

    }
    void OnCloseButtonClicked()
    {

        EnablePlayerInput();
        _questDetail.gameObject.SetActive(false);
        _questListPanel.SetActive(false);
    }

    void OnQuestSlotClicked(Quest quest)
    {
        _questList.SetActive(false);
        _questDetail.gameObject.SetActive(true);
        _questDetail.SetQuestDetail(quest);
    }
    void InitalizeQuestList()
    {
        foreach (Quest quest in Managers.Quest.ActiveQuests)
        {
            GameObject questSlotGO = Managers.Resource.Instantiate("UI/Scene/Quest/QuestSlot", _content.transform);
            UI_QuestSlot questSlot = questSlotGO.GetComponent<UI_QuestSlot>();
            questSlot.SetQuestSlot(quest);
            questSlot.onQuestSlotClicked -= OnQuestSlotClicked;
            questSlot.onQuestSlotClicked += OnQuestSlotClicked;

            questSlot.gameObject.SetActive(true);
        }
      
    }

    public void ReloadeQuestList()
    {
        OnQuestListButtonClicked();
            
        //ClearQuestList();
        //InitalizeQuestList();
    }
    void DisablePlayerInput()
    {
        PlayerController.Instance.PlayerInputDisabled = true;
        Time.timeScale = 0;
    }
    void EnablePlayerInput()
    {
        PlayerController.Instance.PlayerInputDisabled = false;
        Time.timeScale = 1;
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
