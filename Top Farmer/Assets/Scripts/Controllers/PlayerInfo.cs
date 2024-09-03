using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerInfo : ISaveable
{

    public string FarmerName;
    public string FarmName;
    public string FarmerCoin;

    string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; }set { _gameObjectSave = value; } }

    public void Init()
    {
        GameObjectSave = new GameObjectSave();
        ISaveableUniqueID = "PlayerInfo";

        ISaveableRegister();
    }
    public void SetPlayerInfo(string uniqueId, string farmerName, string farmName, string farmerCoin)
    {

        GameObjectSave = new GameObjectSave();

        //_iSaveableUniqueID = uniqueId;
        ISaveableUniqueID = "PlayerInfo";
        FarmerName = farmerName;
        FarmName = farmName;
        FarmerCoin = farmerCoin;
    }

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

        sceneSave._stringDictionary = new Dictionary<string, string>();

        sceneSave._stringDictionary.Add("farmerName", FarmerName);
        sceneSave._stringDictionary.Add("farmName", FarmName);
        sceneSave._stringDictionary.Add("farmerCoin", PlayerController.Instance.PlayerCoin.ToString());

        GameObjectSave.sceneData.Add(Define.PersistentScene,sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave._gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            if(GameObjectSave.sceneData.TryGetValue(Define.PersistentScene, out SceneSave sceneSave))
            {
                if(sceneSave._stringDictionary != null)
                {
                    if (sceneSave._stringDictionary.TryGetValue("farmerName", out string farmerName))
                    {
                        FarmerName = farmerName;
                    }
                    if(sceneSave._stringDictionary.TryGetValue("farmName", out string farmName))
                    {
                        FarmName = farmName;
                    }
                    if (sceneSave._stringDictionary.TryGetValue("farmerCoin", out string farmerCoin))
                    {
                        FarmerCoin = farmerCoin;
                    }
                }
            }
        }
    }


    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required here since the player is on a persistent scene;
    }
    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required here since the player is on a persistent scene;
    }


}
