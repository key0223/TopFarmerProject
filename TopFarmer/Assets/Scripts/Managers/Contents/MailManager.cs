using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailManager
{
    public Queue<string> _mailReceived = new Queue<string>();
    public Queue<string> _mailBox = new Queue<string>();
    public Queue<string> _mailForTomorrow = new Queue<string>();

    public void Init()
    {
        Managers.Event.DayPassedRegistered += OnNewDay;
    }
    public bool HasOrWillReceiveMail(string mailId)
    {
        return _mailReceived.Contains(mailId) || _mailBox.Contains(mailId) || _mailForTomorrow.Contains(mailId);
    }
    public void EnqueueMail(string mailId)
    {
        _mailBox.Enqueue(mailId);
        Managers.Event.UpdateMailBox();
    }
    public string DequeueMail()
    {
        string mailId = "";
        if(_mailBox.Count > 0)
        {
            mailId = _mailBox.Dequeue();
            _mailReceived.Enqueue(mailId);
            Managers.Event.UpdateMailBox();
            return mailId;
        }

        return null;
    }
    public void OnNewDay()
    {
        string mailId1 = TimeManager.Instance.GetCurrentSeasonString() + "_" +
                                                 TimeManager.Instance.GameDay.ToString() + "_" +
                                                 TimeManager.Instance.GameYear.ToString();

        string mailId2 = TimeManager.Instance.GetCurrentSeasonString() + "_" +TimeManager.Instance.GameDay.ToString();
                                               

        if (Managers.Data.StringDict.ContainsKey(mailId1))
        {
            EnqueueMail(mailId1);
        }
        else if (Managers.Data.StringDict.ContainsKey (mailId2))
        {
            EnqueueMail(mailId2);
        }
       
    }

    public void Clear()
    {
        Managers.Event.DayPassedRegistered -= OnNewDay;
    }
    

}
