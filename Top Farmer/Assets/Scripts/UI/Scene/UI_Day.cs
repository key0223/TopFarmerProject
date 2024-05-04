using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Day : UI_Base
{
    enum Texts
    {
        DayOfWeekText,
        MonthText,
        DayText,
        SurvivalStringText,
        SurvivalValueText,
    }
    enum GameObjects
    {
        Weather,
    }

    string[] monthAbbreviations = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
    string[] dayOfWeekAbbreviations = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"};

    public int currentDay { get; private set; }
    public int currentMonth { get; private set; }
    public int currentYear { get; private set; }
    public int currentDayOfWeek { get; private set; }
    public int currentDayOfSurvival { get; private set; }  

    int lastDayOfMonth;


    private Animator _animator;

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        _animator = GetObject((int)GameObjects.Weather).gameObject.GetComponent<Animator>();
        //SetPlayerData();

        Managers.Time.DayPassedRegistered -= OnNextDay;
        Managers.Time.DayPassedRegistered += OnNextDay;

    }

    public void SetPlayerData()
    {
        currentDayOfWeek = Managers.Time.CurrentDayOfWeek;
        currentDay = Managers.Time.CurrentDay;
        currentMonth = Managers.Time.CurrentMonth;
        currentYear = Managers.Time.CurrentYear;
        lastDayOfMonth = DateTime.DaysInMonth(currentYear, currentMonth);

        currentDayOfSurvival = Managers.Time.CurrentDayOfSurvival;

        UpdateUI();
    }


    public void OnNextDay()
    {
        currentDay++;
        currentDayOfWeek++;
        currentDayOfSurvival++;

        if (currentDayOfWeek > 6 || currentDayOfWeek == -1)
            currentDayOfWeek = 0;

        if (currentDay > lastDayOfMonth)
        {
            currentDay = 1;
            currentMonth++;
            if (currentMonth > 11 || currentMonth == -1)
            {
                currentMonth = 0;
                currentYear++;

            }

            lastDayOfMonth = DateTime.DaysInMonth(currentYear, currentMonth);
        }

        UpdateUI();
    }

    public void UpdateAnimation()
    {
        if (_animator == null)
            return;

        switch(Managers.Time.State)
        {
            case DayState.Dawn:
                _animator.Play("Dawn");
                break;
            case DayState.Day:
                _animator.Play("Day");
                break;
            case DayState.Noon:
                _animator.Play("Noon");
                break;
            case DayState.Night:
                _animator.Play("Night");
                break;

        }
    }

    public void UpdateUI()
    {
        GetText((int)Texts.DayOfWeekText).text = dayOfWeekAbbreviations[currentDayOfWeek];
        GetText((int)Texts.MonthText).text = monthAbbreviations[currentMonth];
        GetText((int)Texts.DayText).text = currentDay.ToString();
        GetText((int)Texts.SurvivalValueText).text = currentDayOfSurvival.ToString();
    }
}
