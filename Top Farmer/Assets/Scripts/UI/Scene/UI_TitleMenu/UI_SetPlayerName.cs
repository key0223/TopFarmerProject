using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_SetPlayerName : MonoBehaviour
{
    [SerializeField] InputField _farmerNameInput;
    [SerializeField] InputField _farmNameInput;
    [SerializeField] Button _confirmButton;

    string _farmerName;
    string _farmName;

    private void Awake()
    {
        _farmerName = _farmerNameInput.GetComponent<InputField>().text;
        _farmName = _farmNameInput.GetComponent<InputField>().text;
        _confirmButton.interactable = false;
        _confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        _farmerNameInput.onValueChanged.AddListener(delegate { InputText(); });
        _farmNameInput.onValueChanged.AddListener(delegate { InputText(); });

        gameObject.SetActive(false);
    }

    private void Update()
    {
        _confirmButton.interactable = _farmerName.Length > 0 && _farmName.Length > 0;
    }
    public void InputText()
    {
        _farmerName = _farmerNameInput != null ? _farmerNameInput.text : "";
        _farmName = _farmNameInput != null ? _farmNameInput.text : "";

        _confirmButton.interactable = _farmerName.Length > 0 && _farmName.Length > 0;
    }

    void OnConfirmButtonClicked()
    {
        //string uniqueId = Guid.NewGuid().ToString();
        string uniqueId = _farmerName;
        Managers.PlayerInfo.SetPlayerInfo(uniqueId, _farmerName, _farmName);

        StartCoroutine(CoLoadInitialScene());
    }
    IEnumerator CoLoadInitialScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(Define.PersistentScene);
        op.allowSceneActivation = false;

        float timer = 0;
        while(!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;
            if(op.progress<0.9f)
            {
                Debug.Log("Loading...");
            }
            else
            {
                op.allowSceneActivation = true;
                yield break;
            }
        }
    }
}
