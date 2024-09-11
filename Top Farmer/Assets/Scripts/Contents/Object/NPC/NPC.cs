using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(GenerateGUID))]
public class NPC : MonoBehaviour,ISaveable
{
    string _iSaveableUniqueID;
    GameObjectSave _gameObjectSave;

    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    public GameObjectSave  GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    NPCMovement _npcMovement;

    bool _receivedGift = false;
    public bool ReceivedGift { get { return _receivedGift; } }
    void Awake()
    {
        //ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        ISaveableUniqueID = gameObject.name;
        GameObjectSave = new GameObjectSave();
    }
    void OnEnable()
    {
        ISaveableRegister();
    }
    void OnDisable()
    {
        ISaveableDeregister();
    }
    void Start()
    {
        _npcMovement = GetComponent<NPCMovement>();
    }

    #region Save
    public void ISaveableRegister()
    {
       Managers.Save.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
       Managers.Save.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        GameObjectSave.sceneData.Remove(Define.PersistentScene);

        SceneSave sceneSave = new SceneSave();
        sceneSave._vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave._stringDictionary = new Dictionary<string, string>();

        sceneSave._vector3Dictionary.Add("npcTargetGridPosition", new Vector3Serializable(_npcMovement._npcTargetGridPosition.x, _npcMovement._npcTargetGridPosition.y, _npcMovement._npcTargetGridPosition.z));
        sceneSave._vector3Dictionary.Add("npcTargetWorldPosition", new Vector3Serializable(_npcMovement._npcTargetWorldPosition.x, _npcMovement._npcTargetWorldPosition.y, _npcMovement._npcTargetWorldPosition.z));
        sceneSave._stringDictionary.Add("npcTargetScene", _npcMovement._npcTargetScene.ToString());

        GameObjectSave.sceneData.Add(Define.PersistentScene, sceneSave);
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave._gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            if(GameObjectSave.sceneData.TryGetValue(Define.PersistentScene, out SceneSave sceneSave))
            {
                if(sceneSave._vector3Dictionary != null && sceneSave._stringDictionary != null)
                {
                    if (sceneSave._vector3Dictionary.TryGetValue("npcTargetGridPosition", out Vector3Serializable savedNPCTargetGridPosition))
                    {
                        _npcMovement._npcTargetGridPosition = new Vector3Int((int)savedNPCTargetGridPosition.x ,(int)savedNPCTargetGridPosition.y,(int)savedNPCTargetGridPosition.z);
                        _npcMovement._npcCurrentGridPosition = _npcMovement._npcTargetGridPosition;
                    }

                    if(sceneSave._vector3Dictionary.TryGetValue("npcTargetWorldPosition", out Vector3Serializable savedNPCTargetWorldPosition))
                    {
                        _npcMovement._npcTargetWorldPosition = new Vector3(savedNPCTargetWorldPosition.x, savedNPCTargetWorldPosition.y, savedNPCTargetWorldPosition.z);
                        transform.position = _npcMovement._npcTargetWorldPosition;
                    }

                    if(sceneSave._stringDictionary.TryGetValue("npcTargetScene", out string saveTargetScene ))
                    {
                        if(Enum.TryParse<Define.Scene>(saveTargetScene, out Define.Scene sceneName))
                        {
                            _npcMovement._npcTargetScene = sceneName;
                            _npcMovement._npcCurrentScene = _npcMovement._npcTargetScene;
                        }
                    }

                    _npcMovement.CancelNPCMovement();

                }
                    
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required here since on persistent scene
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required here since on persistent scene
    }
    #endregion

   
}
