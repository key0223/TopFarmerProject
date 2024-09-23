using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_SavefileButton : MonoBehaviour
{
    [SerializeField] Text _farmerNameText;
    [SerializeField] Text _farmNameText;
    [SerializeField] Text _farmerCoinText;

    GameSave _gameSave;
    public void SetButton(GameSave gameSave)
    {
        _gameSave = gameSave;
        if (gameSave._gameObjectData.TryGetValue($"PlayerInfo", out GameObjectSave gameObjectSave))
        {
            if (gameObjectSave.sceneData.TryGetValue(Define.PersistentScene, out SceneSave sceneSave))
            {
                if (sceneSave._stringDictionary.TryGetValue("farmerName", out string farmerName))
                {
                    _farmerNameText.text = farmerName;
                }
                if (sceneSave._stringDictionary.TryGetValue("farmName", out string farmName))
                {
                    _farmNameText.text = farmName;
                }
                if (sceneSave._stringDictionary.TryGetValue("farmerCoin", out string farmerCoin))
                {
                    _farmerCoinText.text = farmerCoin;
                }
            }
        }

      Button button  = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnButtonSelected);
    }
   
    public void OnButtonSelected()
    {
        Managers.PlayerInfo.SetPlayerInfo(_farmerNameText.text, _farmerNameText.text, _farmNameText.text,_farmerCoinText.text);
        StartCoroutine(CoLoadScene());

    }

    IEnumerator CoLoadScene()
    {
        //yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(Define.PersistentScene);
     
        op.allowSceneActivation = false;

        float timer = 0;
        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                Debug.Log("Loading...");
                yield return null;
            }
            else
            {
                Managers.PlayerInfo.SetPlayerInfo(_farmerNameText.text, _farmerNameText.text, _farmNameText.text, _farmerCoinText.text);
                op.allowSceneActivation = true;
                yield return null;
            }
        }
      
    }
}

