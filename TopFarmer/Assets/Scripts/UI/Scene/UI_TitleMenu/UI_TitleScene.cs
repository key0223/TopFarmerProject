using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TitleScene : MonoBehaviour
{
    [SerializeField] GameObject _mainMenu;

    [SerializeField] GameObject[] _menuTabs = null;
    [SerializeField] Button[] _menuButtons = null;

    bool _mainMenuOn = true;

    public bool MainMenuOn { get { return _mainMenuOn; } set { _mainMenuOn = value; } }

    private void Awake()
    {
        _mainMenu.SetActive(true);
    }

    void EnableMainMenu()
    {
        _mainMenuOn = true;
        _mainMenu.SetActive(true);


    }
    void DisableMainMenu()
    {
        _mainMenuOn = false;
        _mainMenu.SetActive(false);
    }

    public void OnNewGameButtonClicked(int tabNum)
    {
        for (int i = 0; i < _menuTabs.Length; i++)
        {
            if(i ==tabNum)
            {
                _menuTabs[i].SetActive(true);
                UISlide slide = _menuTabs[i].GetComponent<UISlide>();
                slide.ShowUI();

                _mainMenu.SetActive(false);
            }
        }
    }
    public void OnLoadButtonClicked(int tabNum)
    {
        for (int i = 0; i < _menuTabs.Length; i++)
        {
            if (i == tabNum)
            {
                _menuTabs[i].SetActive(true);
                UISlide slide = _menuTabs[i].GetComponent<UISlide>();
                slide.ShowUI();

                _mainMenu.SetActive(false);
            }
        }
    }

}
