using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class QuestManager : ISaveable
{
    List<Quest> _activeQuests = new List<Quest>();

    public List<Quest> ActiveQuests {  get { return _activeQuests; } }

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    public void Init()
    {
        ISaveableRegister();
        ISaveableUniqueID = "Quests";
        GameObjectSave = new GameObjectSave();
    }
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
    public void Clear()
    {
        ISaveableDeregister();
    }
    #region Save
    public GameObjectSave ISaveableSave()
    {
        SceneSave sceneSave = new SceneSave();

        GameObjectSave.sceneData.Remove(PersistentScene);

        List<QuestSave> questSaveList = new List<QuestSave>();

        foreach(Quest quest in _activeQuests)
        {
            questSaveList.Add(quest.SaveQuest());
        }

        sceneSave._activeQuests = questSaveList;

        GameObjectSave.sceneData.Add(PersistentScene,sceneSave);

        return GameObjectSave;

    }
   
    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave._gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            if(gameObjectSave.sceneData.TryGetValue(PersistentScene, out SceneSave sceneSave))
            {
                if(sceneSave._activeQuests !=null)
                {
                    _activeQuests = new List<Quest>();

                    foreach(QuestSave questSave in sceneSave._activeQuests)
                    {
                        Quest quest = Quest.LoadQuest(questSave);
                        if(quest != null)
                        {
                            AcceptQuest(quest);
                        }
                    }
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
