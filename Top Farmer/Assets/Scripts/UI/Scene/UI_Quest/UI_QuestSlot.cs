using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestSlot : MonoBehaviour
{
    public delegate void QuestSlotCilckedHandler(Quest quest);
    public event QuestSlotCilckedHandler onQuestSlotClicked;

    [SerializeField] Text _questTitle;
    [SerializeField] Image _completeImage;
    Button _slotButton;

    public Quest Quest {  get; private set; }

    private void Awake()
    {
        _slotButton = GetComponent<Button>();
        _slotButton.onClick.AddListener(()=> OnSlotButtonClicked());
    }
    public void SetQuestSlot(Quest quest)
    {
        _completeImage.gameObject.SetActive(false);

        Quest = quest;
        _questTitle.text = quest.QuestTitle;

        if(quest.QuestState == Define.QuestState.WatingForCompletion )
        {
            _completeImage.gameObject.SetActive(true);
        }
        
    }

    void OnSlotButtonClicked()
    {
        onQuestSlotClicked(Quest);
        gameObject.SetActive(false);
    }
}
