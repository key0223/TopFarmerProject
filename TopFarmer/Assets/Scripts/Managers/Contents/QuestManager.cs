using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
    List<Quest> _activeQuests = new List<Quest>();

    public List<Quest> ActiveQuests {  get { return _activeQuests; } }

    public void AcceptQuest(Quest quest)
    {
        quest.onCompleted += OnQuestCompleted;
        _activeQuests.Add(quest);
        quest.OnRegister();
    }

    void OnQuestCompleted(Quest quest)
    {
        _activeQuests.Remove(quest);
    }
}
