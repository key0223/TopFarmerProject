using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MailBox : MonoBehaviour
{
    [SerializeField] GameObject _exclamationMark;

    private void OnEnable()
    {
        Managers.Event.UpdateMailBoxEvent -= UpdateExclamationMark;
        Managers.Event.UpdateMailBoxEvent += UpdateExclamationMark;
    }
    private void OnDisable()
    {
        Managers.Event.UpdateMailBoxEvent -= UpdateExclamationMark;
    }
    void UpdateExclamationMark()
    {
        if (Managers.Mail._mailBox.Count > 0)
        {
            _exclamationMark.SetActive(true);  // 마크 활성화
        }
        else
        {
            _exclamationMark.SetActive(false);  // 마크 비활성화
        }
    }

   

    //TODO: OnClick Method 

    [ContextMenu("Make Daily Quest")]
    void MakeDailyQuest()
    {
        Quest newDailyQuest = Quest.MakeQuest();
        Managers.Quest.AcceptQuest(newDailyQuest);
    }
}
