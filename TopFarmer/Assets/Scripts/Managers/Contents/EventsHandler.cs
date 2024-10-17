using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EventsHandler 
{

    public event Action<InventoryType, List<InventoryItem>> UpdateInventoryEvent;
    public event Action UpdatePlayerCoinEvent;

    public event Action InstantiateCropPrefabsEvent;
    public event Action DropSelectedItemEvent;
    public event Action<MoveDir, ItemType> ToolAnimationEvent;
    public event Action RemoveSelectedItemFromInventoryEvent;

    public event Action UpdateMailBoxEvent;

    #region Time Event
    public event Action MinutePassedRegisterd;
    public event Action HourPassedRegistered;
    public event Action DayPassedRegistered;
    public event Action SeasonPassedRegistered;
    public event Action YearPassedRegistered;
    #endregion

    #region Scene
    public event Action BeforeSceneUnloadFadeOutEvent;
    public event Action BeforeSceneUnloadEvent;
    public event Action AfterSceneLoadEvent;
    public event Action AfterSceneLoadFadeInEvent;
    #endregion

    public void UpdateInventory(InventoryType invenType,List<InventoryItem> inventory)
    {
        UpdateInventoryEvent?.Invoke(invenType, inventory);
    }
    public void UpdateCoin()
    {
        UpdatePlayerCoinEvent?.Invoke();
    }
    public void InstantiateCrops()
    {
        InstantiateCropPrefabsEvent?.Invoke();
    }
    public void DropSelectedItem()
    {
        DropSelectedItemEvent?.Invoke();
    }

    public void StartToolAnimation(MoveDir moveDir, ItemType itemType)
    {
        ToolAnimationEvent?.Invoke(moveDir, itemType);
    }

    public void RemoveSelectedItemFromInventory()
    {
        RemoveSelectedItemFromInventoryEvent?.Invoke();
    }

    public void UpdateMailBox()
    {
        UpdateMailBoxEvent?.Invoke();
    }
    #region Time Events
    public void CallMinutePassedEvent()
    {
        MinutePassedRegisterd?.Invoke();
    }
    public  void CallHourPassedEvent()
    {
        HourPassedRegistered?.Invoke();
    }
    public  void CallDayPassedEvent()
    {
        DayPassedRegistered?.Invoke();
    }
    public  void CallSeasonPassedEvent()
    {
        SeasonPassedRegistered?.Invoke();
    }
    public  void CallYearPassedEvent()
    {
        YearPassedRegistered?.Invoke();
    }
    #endregion

    #region Scene Events
    public void CallBeforeSceneUnloadFadeOutEvent()
    {
        BeforeSceneUnloadFadeOutEvent?.Invoke();
    }
    public void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    public void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }
    public void CallAfterSceneLoadFadeInEvent()
    {
        AfterSceneLoadFadeInEvent?.Invoke();
    }
    #endregion
}
