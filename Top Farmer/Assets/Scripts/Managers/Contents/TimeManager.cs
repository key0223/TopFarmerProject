using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TimeManager
{
    #region Event
    public delegate void MinutePassedRegisterHandler();
    public event MinutePassedRegisterHandler MinutePassedRegisterd;

    public delegate void HourPassedRegisterHandler();
    public event HourPassedRegisterHandler HourPassedRegistered;

    public delegate void DayPassedRegisteredHandler();
    public event DayPassedRegisteredHandler DayPassedRegistered;
    #endregion

    bool _init = false;
    private const float _timeRatio = 0.3f; // 인게임 10분 = 실제 시간 7초

    public int CurrentDayOfSurvival { get; private set; }
    public float CurrentMinute { get; private set; }
    public int CurrentHour { get; private set; } 
    public int CurrentDay { get; private set; } 

    public int CurrentDayOfWeek { get; private set; }
    public int CurrentMonth { get; private set; } 
    public int CurrentYear { get; private set; }
   
    private float _realTime = 0.0f;
    //UI_GameScene gameSceneUI;

    private DayState _state = DayState.Dawn;
    public DayState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;
            _state = value;

            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            gameSceneUI.DayUI.UpdateAnimation();
            gameSceneUI.LightUI.UpdateAlpha();

        }
    }

    void SetTime()
    {
        GameTime gameTime = Managers.SaveLoad.CurrentGameData.gameTime;
        State = gameTime.dayState;
        CurrentDayOfSurvival = gameTime.dayOfSurvival;
        CurrentMinute = gameTime.minute;
        CurrentHour = gameTime.hour;
        CurrentDay = gameTime.day;
        CurrentDayOfWeek = gameTime.week;
        CurrentMonth = gameTime.month;
        CurrentYear = gameTime.year;
    }
    public void Init()
    {
        SetTime();
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        gameSceneUI.DayUI.SetPlayerData();
        gameSceneUI.DayUI.UpdateAnimation();
        gameSceneUI.LightUI.UpdateAlpha();
        UpdateState();
    }

    void UpdateState()
    {
        if (CurrentHour >= 5 && CurrentHour < 7)
        {
            State = DayState.Dawn;
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            gameSceneUI.NightUIOn();
            Debug.Log("Dawn");
        }
        else if (CurrentHour >= 7 && CurrentHour < 17)
        {
            State = DayState.Day;
            Debug.Log("Day");

        }
        else if (CurrentHour >= 17 && CurrentHour < 19)
        {
            State = DayState.Noon;
            Debug.Log("Noon");

        }
        else
        {
            State = DayState.Night;
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            gameSceneUI.NightUIOff();
            Debug.Log("Night");
        }

    }
    public void Update()
    {
        _realTime += Time.deltaTime;

        if (_realTime >= _timeRatio)
        {
            _realTime = 0.0f;
            CurrentMinute += 10.0f;
            MinutePassedRegisterd?.Invoke();

            if (CurrentMinute >= 60.0f)
            {
                CurrentMinute = 0.0f;
                CurrentHour++;
                HourPassedRegistered?.Invoke();
                UpdateState();

                if (CurrentHour >= 24)
                {
                    CurrentHour = 0;
                    CurrentDay++;
                    DayPassedRegistered?.Invoke();
                }
            }
        }
    }

    
}
