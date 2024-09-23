using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[Serializable]
public class NPCScheduleEvent
{
    public int _hour;
    public int _minute;
    public int _priority;
    public int _day;
    public Weather _weather;
    public Season _season;
    public Scene _toSceneName;
    public GridCoordinate _toGridCoordinate;
    public MoveDir _dir;
    public AnimationClip _animationAtDestination;

    public int Time
    {
        get
        {
            return (_hour * 100) + _minute;
        }
    }


    public NPCScheduleEvent(int hour, int minute, int priority, int day, Weather weather, Season season, Scene toSceneName, GridCoordinate toGridCoordinate, AnimationClip animationAtDestination)
    {
        _hour = hour;
        _minute = minute;
        _priority = priority;
        _day = day;
        _weather = weather;
        _season = season;
        _toSceneName = toSceneName;
        _toGridCoordinate = toGridCoordinate;
        _animationAtDestination = animationAtDestination;
    }

    public NPCScheduleEvent()
    {

    }

    public override string ToString()
    {
        return $"Time: {Time}, Priority: {_priority}, Day: {_day} Weather: {_weather}, Season: {_season}";
    }
}
