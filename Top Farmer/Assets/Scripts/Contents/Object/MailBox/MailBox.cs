using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailBox : MonoBehaviour
{
    Queue<IMailItem> _mails = new Queue<IMailItem>();
    public Queue<IMailItem> Mails { get { return _mails; } }

    [SerializeField]GameObject _uiTrigger;
    [SerializeField] GameObject _exclamationMark;

    bool _isLoaded = false;
    private void Start()
    {
        ClearQueue();
        GetQuestMails();
    }

    private void OnEnable()
    {
        if(!_isLoaded)
        {
            GetQuestMails();
        }
    }
    public void AddToMailBox(IMailItem mail)
    {
        _mails.Enqueue(mail);
        UpdateExclamiationMark();
    }
    public IMailItem GetMail()
    {
        IMailItem mail = null;
        if ( _mails.Count>0)
        {
            mail = _mails.Dequeue();
        }
        UpdateExclamiationMark();
        return mail;
    }

    void GetQuestMails()
    {
        List<QuestData> questDatas = Managers.Quest.GetAcceptableQuestData();

        if(questDatas!=null )
        {
            foreach( QuestData data in questDatas )
            {
                QuestMailItem questMail = new QuestMailItem(data);
                AddToMailBox((QuestMailItem)questMail);
            }
        }

        _isLoaded = true;
    }

    void UpdateExclamiationMark()
    {
        _exclamationMark.SetActive(_mails.Count > 0);
        _uiTrigger.SetActive(_mails.Count > 0);
        Debug.Log(_mails.Count.ToString());
    }

    void ClearQueue()
    {
        _mails.Clear();
        _isLoaded = false;
    }
}