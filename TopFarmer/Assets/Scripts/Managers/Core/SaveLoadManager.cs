using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager
{
    public GameSave _gameSave;
    public List<ISaveable> iSaveableObjectList;

    public void Init()
    {
        iSaveableObjectList = new List<ISaveable>();
    }
    
    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + $"/{Managers.PlayerInfo.FarmerName}.dat"))
        {
            _gameSave = new GameSave();

            FileStream file = File.Open(Application.persistentDataPath + $"/{Managers.PlayerInfo.FarmerName}.dat", FileMode.Open);
            _gameSave  = (GameSave)bf.Deserialize(file);

            for (int i= iSaveableObjectList.Count -1;  i> -1; i--)
            {
                if (_gameSave._gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(_gameSave);
                }
                else
                {
                    Component component = (Component)iSaveableObjectList[i];
                    Managers.Resource.Destroy(component.gameObject);
                }
            }

            file.Close();

            Debug.Log("Data Loaded");
        }
    
    }

    public void LoadData(GameSave gameSave)
    {
        _gameSave = gameSave;

        for (int i = iSaveableObjectList.Count - 1; i > -1; i--)
        {
            if (_gameSave._gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
            {
                iSaveableObjectList[i].ISaveableLoad(_gameSave);
            }
            else
            {
                Component component = (Component)iSaveableObjectList[i];
                Managers.Resource.Destroy(component.gameObject);
            }
        }

        Debug.Log("Data Loaded");

    }

    public List<string> GetAllSaveFiles()
    {
        string path = Application.persistentDataPath;
        string[] files = Directory.GetFiles(path,"*.dat");
        List<string> saveFiles = new List<string>();

        foreach(string file in files)
        {
            saveFiles.Add(Path.GetFileNameWithoutExtension(file));
        }

        return saveFiles;
    }

    public void SaveDataToFile()
    {
        _gameSave = new GameSave();

        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            _gameSave._gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
        }

        string saveFileName = Managers.PlayerInfo.FarmerName;

        BinaryFormatter bf = new BinaryFormatter();

        //FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);

        using(FileStream file = File.Open(Application.persistentDataPath + $"/{saveFileName}.dat", FileMode.Create))
        {
            bf.Serialize(file, _gameSave);
        }
       
        Debug.Log("Data Saved");

    }
   
    public void StoreCurrentSceneData()
    {
        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
