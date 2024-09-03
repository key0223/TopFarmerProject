using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestSlot : MonoBehaviour
{
    [SerializeField] Text _questTitle;

    public Quest Quest {  get; private set; }

    public void SetQuestSlot(Quest quest)
    {
        Quest = quest;
        _questTitle.text = quest.QuestTitle;

    }
}
