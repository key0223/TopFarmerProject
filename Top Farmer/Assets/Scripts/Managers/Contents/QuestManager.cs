using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    List<Quest> _activeQuests = new List<Quest>();

    public List<Quest> ActiveQuests {  get { return _activeQuests; } }

    public void AcceptQuest(Quest quest)
    {
        _activeQuests.Add(quest);
    }
}
