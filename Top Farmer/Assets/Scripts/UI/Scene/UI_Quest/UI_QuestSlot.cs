using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestSlot : MonoBehaviour
{
    public delegate void QuestSlotCilckedHandler(Quest quest);
    public event QuestSlotCilckedHandler onQuestSlotClicked;

    [SerializeField] Text _questTitle;
    Button _slotButton;

    public Quest Quest {  get; private set; }

    private void Awake()
    {
        _slotButton = GetComponent<Button>();
        _slotButton.onClick.AddListener(()=> OnSlotButtonClicked());
    }
    public void SetQuestSlot(Quest quest)
    {
        Quest = quest;
    }

    void OnSlotButtonClicked()
    {
        onQuestSlotClicked(Quest);
        gameObject.SetActive(false);
    }
}
