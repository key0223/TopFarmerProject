using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailManager
{
    public Queue<string> _mailReceived = new Queue<string>();
    public Queue<string> _mailBox = new Queue<string>();
    public Queue<string> _mailForTomorrow = new Queue<string>();

    public bool HasOrWillReceiveMail(string mailId)
    {
        return _mailReceived.Contains(mailId) || _mailBox.Contains(mailId) || _mailForTomorrow.Contains(mailId);
    }

    public void OnNewDay()
    {
        string mailId1 = TimeManager.Instance.GetCurrentSeasonString() + "_" +
                                                 TimeManager.Instance.GameDay.ToString() + "_" +
                                                 TimeManager.Instance.GameYear.ToString();

        string mailId2 = TimeManager.Instance.GetCurrentSeasonString() + "_" +TimeManager.Instance.GameDay.ToString();
                                               

        if (Managers.Data.StringDict.ContainsKey(mailId1))
        {
            _mailBox.Enqueue(mailId1);
        }
        else if (Managers.Data.StringDict.ContainsKey (mailId2))
        {
            _mailBox.Enqueue(mailId2);
        }
       
    }
}
