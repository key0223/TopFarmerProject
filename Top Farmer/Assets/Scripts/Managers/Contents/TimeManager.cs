using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TimeManager :SingletonMonobehaviour<TimeManager>, ISaveable
{
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    //  Starting value
    public Season GameSeason { get; private set; } = Season.SPRING;
    public int GameYear { get; private set; } = 1;
    public int GameDay { get; private set; } = 1;
    public int GameHour { get; private set; } = 6;
    public int GameMinute { get; private set; } = 0;
    public int GameSecond { get; private set; } = 0;

    public string GameDayOfWeek { get; private set; } = "Mon";
  

    bool _gameClockPaused = false;
    float _gameTick = 0f;

    protected override void Awake()
    {
        base.Awake();

        //ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        ISaveableUniqueID = "TimeManager";
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        Managers.Event.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        Managers.Event.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }
    private void OnDisable()
    {
        ISaveableDeregister();

        Managers.Event.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        Managers.Event.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    void Start()
    {
        Managers.Event.CallMinutePassedEvent();
    }
    //public void Init()
    //{
    //    Managers.Event.CallMinutePassedEvent();
    //}
    private void Update()
    {
        if (!_gameClockPaused)
        {
            GameTick();
        }
    }
    void GameTick()
    {
        _gameTick += Time.deltaTime;
        if (_gameTick >= SecondsPerGameSecond)
        {
            _gameTick -= SecondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    void UpdateGameSecond()
    {
        GameSecond++;

        if (GameSecond > 59)
        {
            GameSecond = 0;
            GameMinute++;

            if (GameMinute > 59)
            {
                GameMinute = 0;
                GameHour++;

                if (GameHour > 23)
                {
                    GameHour = 0;
                    GameDay++;

                    if (GameDay > 30)
                    {
                        GameDay = 1;

                        int gs = (int)GameSeason;
                        gs++;

                        GameSeason = (Season)gs;

                        if (gs > 4)
                        {
                            gs = 1;
                            GameSeason = (Season)gs;

                            GameYear++;

                            if (GameYear > 9999)
                                GameYear = 1;

                            Managers.Event.CallYearPassedEvent();
                        }
                        Managers.Event.CallSeasonPassedEvent();
                    }
                    GameDayOfWeek = GetDayOfWeek();
                    Managers.Event.CallDayPassedEvent();
                }
                Managers.Event.CallHourPassedEvent();
            }
            Managers.Event.CallMinutePassedEvent();
        }
        // Call to advance game second event would go here if required
    }

    private string GetDayOfWeek()
    {
        int totalDays = (((int)GameSeason) * 30) + GameDay;
        int dayOfWeek = totalDays % 7;

        switch (dayOfWeek)
        {
            case 1:
                return "Mon";

            case 2:
                return "Tue";

            case 3:
                return "Wed";

            case 4:
                return "Thu";

            case 5:
                return "Fri";

            case 6:
                return "Sat";

            case 0:
                return "Sun";

            default:
                return "";
        }
    }

    public TimeSpan GetGameTime()
    {
        TimeSpan gameTime = new TimeSpan(GameHour, GameMinute, GameSecond);

        return gameTime;
    }

    public void TestAdvancedMinute()
    {
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond();

        }
    }

    public void TestAdvancedDay()
    {
        for(int i = 0;i < 86400;i++)
        {
            UpdateGameSecond();
        }
    }


    private void BeforeSceneUnloadFadeOut()
    {
        _gameClockPaused = true;
    }

    private void AfterSceneLoadFadeIn()
    {
        _gameClockPaused = false;
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
        // Delete existing scene save if exists
        GameObjectSave.sceneData.Remove(PersistentScene);

        // Create new scene save
        SceneSave sceneSave = new SceneSave();

        // Create new int dictionary
        sceneSave._intDictionary = new Dictionary<string, int>();

        // Create new string dictionary
        sceneSave._stringDictionary = new Dictionary<string, string>();

        // Add values to the int dictionary
        sceneSave._intDictionary.Add("gameYear", GameYear);
        sceneSave._intDictionary.Add("gameDay", GameDay);
        sceneSave._intDictionary.Add("gameHour", GameHour);
        sceneSave._intDictionary.Add("gameMinute", GameMinute);
        sceneSave._intDictionary.Add("gameSecond", GameSecond);

        // Add values to the string dictionary
        sceneSave._stringDictionary.Add("gameDayOfWeek", GameDayOfWeek);
        sceneSave._stringDictionary.Add("gameSeason", GameSeason.ToString());

        // Add scene save to game object for persistent scene
        GameObjectSave.sceneData.Add(PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        // Get saved gameobject from gameSave data
        if (gameSave._gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Get savedscene data for gameObject
            if (GameObjectSave.sceneData.TryGetValue(PersistentScene, out SceneSave sceneSave))
            {
                // if int and string dictionaries are found
                if (sceneSave._intDictionary != null && sceneSave._stringDictionary != null)
                {
                    // populate saved int values
                    if (sceneSave._intDictionary.TryGetValue("gameYear", out int savedGameYear))
                        GameYear = savedGameYear;

                    if (sceneSave._intDictionary.TryGetValue("gameDay", out int savedGameDay))
                        GameDay = savedGameDay;

                    if (sceneSave._intDictionary.TryGetValue("gameHour", out int savedGameHour))
                        GameHour = savedGameHour;

                    if (sceneSave._intDictionary.TryGetValue("gameMinute", out int savedGameMinute))
                        GameMinute = savedGameMinute;

                    if (sceneSave._intDictionary.TryGetValue("gameSecond", out int savedGameSecond))
                        GameSecond = savedGameSecond;

                    // populate string saved values
                    if (sceneSave._stringDictionary.TryGetValue("gameDayOfWeek", out string savedGameDayOfWeek))
                        GameDayOfWeek = savedGameDayOfWeek;

                    if (sceneSave._stringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                    {
                        if (Enum.TryParse<Season>(savedGameSeason, out Season season))
                        {
                            GameSeason = season;
                        }
                    }

                    // Zero gametick
                    _gameTick = 0f;

                    // Trigger advance minute event
                    Managers.Event.CallMinutePassedEvent();
                    //EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                    // Refresh game clock
                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required here since Time Manager is running on the persistent scene
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required here since Time Manager is running on the persistent scene
    }
}
