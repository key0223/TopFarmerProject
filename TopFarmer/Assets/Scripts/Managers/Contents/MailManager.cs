using System.Collections.Generic;
using static Define;

public class MailManager : ISaveable
{
    public Queue<string> _mailReceived = new Queue<string>();
    public Queue<string> _mailBox = new Queue<string>();
    public Queue<string> _mailForTomorrow = new Queue<string>();

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    public void Init()
    {
        ISaveableRegister();
        ISaveableUniqueID = "Mails";
        GameObjectSave = new GameObjectSave();
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
            _mailReceived.Enqueue(mailId); // 메일을 받음 처리합니다.
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
        ISaveableDeregister();
    }

    #region Save
    public GameObjectSave ISaveableSave()
    {
        SceneSave sceneSave = new SceneSave();

        GameObjectSave.sceneData.Remove(PersistentScene);
        Dictionary<string, string[]> mailData = new Dictionary<string, string[]>();
        mailData["mailReceived"] = _mailReceived.ToArray();
        mailData["mailBox"] = _mailBox.ToArray();
        mailData["mailForTomorrow"] = _mailForTomorrow.ToArray();

        sceneSave._mails = mailData;

        GameObjectSave.sceneData.Add(PersistentScene, sceneSave);

        return GameObjectSave;


    }
    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave._gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            if (gameObjectSave.sceneData.TryGetValue(PersistentScene, out SceneSave sceneSave))
            {
                if (sceneSave._mails != null)
                {
                    _mailReceived = new Queue<string>(sceneSave._mails["mailReceived"]);
                    _mailBox = new Queue<string>(sceneSave._mails["mailBox"]);
                    _mailForTomorrow = new Queue<string>(sceneSave._mails["mailForTomorrow"]);
                }
            }
        }
    }
   

    public void ISaveableRegister()
    {
        Managers.Save.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        Managers.Save.iSaveableObjectList.Remove(this);
    }

    public void ISaveableStoreScene(string sceneName)
    {
    }

    public void ISaveableRestoreScene(string sceneName)
    {
    }
    #endregion
}
