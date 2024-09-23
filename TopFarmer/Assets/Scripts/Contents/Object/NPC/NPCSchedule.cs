using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    [SerializeField] SO_NPCScheduleEventList _soNPCScheduleEventList = null;
    SortedSet<NPCScheduleEvent> _npcScheduleEventSet;
    NPCPath _npcPath;

    private void Awake()
    {
        _npcScheduleEventSet = new SortedSet<NPCScheduleEvent>(new NPCScheduleEventSort());

        foreach(NPCScheduleEvent npcScheduleEvent in _soNPCScheduleEventList.nPCScheduleEventList)
        {
            _npcScheduleEventSet.Add(npcScheduleEvent);
        }

        _npcPath = GetComponent<NPCPath>();
    }

    private void OnEnable()
    {
        Managers.Event.MinutePassedRegisterd += GameTimeSystem_MinutePassed;
    }
    private void OnDisable()
    {
        Managers.Event.MinutePassedRegisterd -= GameTimeSystem_MinutePassed;
    }

    void GameTimeSystem_MinutePassed()
    {
        // Update time
        Season gameSeason = TimeManager.Instance.GameSeason;
        int gameYear = TimeManager.Instance.GameYear;
        string gameDayOfWeek = TimeManager.Instance.GameDayOfWeek;
        int gameDay = TimeManager.Instance.GameDay;
        int gameHour = TimeManager.Instance.GameHour;
        int gameMinute = TimeManager.Instance.GameMinute;
        int gameSecond = TimeManager.Instance.GameSecond;

        int time = (gameHour * 100) + gameMinute;

        NPCScheduleEvent foundNPCScheduleEvent = null;

        foreach(NPCScheduleEvent npcScheduleEvent in _npcScheduleEventSet)
        {
            if(npcScheduleEvent.Time == time)
            {
                if (npcScheduleEvent._day != 0 && npcScheduleEvent._day != gameDay)
                    continue;
                if (npcScheduleEvent._season != Season.NONE && npcScheduleEvent._season != gameSeason)
                    continue;

                if (npcScheduleEvent._weather != Weather.NONE && npcScheduleEvent._weather != Managers.Instance._currentWeather)
                    continue;

                foundNPCScheduleEvent = npcScheduleEvent;
                break;

            }
            else if( npcScheduleEvent.Time> time)
            {
                break;
            }
        }

        if(foundNPCScheduleEvent != null)
        {
            _npcPath.BuildPath(foundNPCScheduleEvent);
        }
    }
}
