using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
  
    bool _pauseMeunOn = false;

    [SerializeField] GameObject _pauseMenu = null;
    [SerializeField] UI_InventoryBar _inventoryBar = null;
    [SerializeField] UI_TabInventory _tabInventory = null;
    [SerializeField] GameObject[] _menuTabs = null;
    [SerializeField] Button[] _menuButton = null;

    public bool PauseMenuOn { get { return _pauseMeunOn; } set { _pauseMeunOn = value; } }


    private void Awake()
    {
        _pauseMenu.SetActive(false);
    }
    private void Update()
    {
        PauseMenu();
    }

     void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
       
    }

    void EnablePauseMenu()
    {
        _inventoryBar.DestoryCurrentlyDraggedItems();
        _inventoryBar.ClearCurrentlySelectedItems();

        PauseMenuOn = true;
        PlayerController.Instance.PlayerInputDisabled = true;
        Time.timeScale = 0;
        _pauseMenu.SetActive(true);

        System.GC.Collect();

        HighlightButtonForSelectedTab();
    }

    void DisablePauseMenu()
    {
        _tabInventory.DestroyCurrentlyDraggedItems();

        PauseMenuOn = false;
        PlayerController.Instance.PlayerInputDisabled = false;
        Time.timeScale = 1;
        _pauseMenu.SetActive(false);
    }

    void HighlightButtonForSelectedTab()
    {
        for (int i = 0; i < _menuTabs.Length; i++)
        {
            if (_menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(_menuButton[i]);
            }
            else
            {
                SetButtonColorToInactive(_menuButton[i]);
            }
        }
    }

    void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.pressedColor;
        button.colors = colors;
    }
    void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.disabledColor;
        button.colors = colors;
    }

    public void SwitchPauseMenuTab(int tabNum)
    {
        for (int i = 0; i < _menuTabs.Length; i++)
        {
            if(i != tabNum)
            {
                _menuTabs[i].SetActive(false);
            }
            else
            {
                _menuTabs[i].SetActive(true);
            }
        }

        HighlightButtonForSelectedTab();
    }


    public void SaveGame()
    {
        Managers.Save.SaveDataToFile();
    }
    public void LoadGame()
    {
        Managers.Save.LoadDataFromFile();
    }
    public void QuitGame()
    {
        Application.Quit();
    }

   
}
