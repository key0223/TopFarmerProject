using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class UI_SelectSaveFile : MonoBehaviour
{
    [SerializeField] Transform _container;

    GameSave _gameSave;

    private void Start()
    {
        PopulateSaveFileButtons();
    }
    void PopulateSaveFileButtons()
    {
        List<string> saveFiles = Managers.Save.GetAllSaveFiles();

        foreach (string saveFile in saveFiles)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/SavefileButtonPrefab",_container);
            UI_SavefileButton button  = go.GetComponent<UI_SavefileButton>();


            BinaryFormatter bf = new BinaryFormatter();
            if (File.Exists(Application.persistentDataPath +$"/{saveFile}.dat"))
            {
                _gameSave = new GameSave();
                using (FileStream file = File.Open(Application.persistentDataPath + $"/{saveFile}.dat", FileMode.Open))
                {
                    _gameSave = (GameSave)bf.Deserialize(file);
                    button.SetButton(_gameSave);

                }
            }
        }
    }
}
