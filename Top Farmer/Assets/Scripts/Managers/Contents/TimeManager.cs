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

    private float _currentMinutes = 0;
    public int CurrentHours { get; private set; } = 18;

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

    public void Init()
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        gameSceneUI.DayUI.UpdateAnimation();
        gameSceneUI.LightUI.UpdateAlpha();
        UpdateState();
    }

    void UpdateState()
    {
        if (CurrentHours >= 5 && CurrentHours < 7)
        {
            State = DayState.Dawn;
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            gameSceneUI.NightUIOn();
            Debug.Log("Dawn");
        }
        else if (CurrentHours >= 7 && CurrentHours < 17)
        {
            State = DayState.Day;
            Debug.Log("Day");

        }
        else if (CurrentHours >= 17 && CurrentHours < 19)
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
            _currentMinutes += 10.0f;
            MinutePassedRegisterd?.Invoke();

            if (_currentMinutes >= 60.0f)
            {
                _currentMinutes = 0.0f;
                CurrentHours++;
                HourPassedRegistered?.Invoke();
                UpdateState();

                if (CurrentHours >= 24)
                {
                    DayPassedRegistered?.Invoke();
                    CurrentHours = 0;
                }
            }
        }
    }

    
}
