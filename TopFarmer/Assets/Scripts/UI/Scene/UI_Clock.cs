using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Clock :MonoBehaviour
{
    [SerializeField] Text _timeText;
    [SerializeField] Text _dateText;

    [SerializeField] Image _dayHand;
    [SerializeField] Image _seasonImage;

    int _minValue = 0;
    int _maxValue = 180;

    float _totlaTime = 18 * 60; // 분 단위 계산

    Season _gameSeason;
    public Season GameSeason
    {
        get { return _gameSeason; }
        set
        {
            if (_gameSeason == value)
                return;

            _gameSeason = value;
            UpdateSeasonSprite();
        }
    }




    private void OnEnable()
    {
        Managers.Event.MinutePassedRegisterd += UpdateGameTime;
    }

    private void OnDisable()
    {
        Managers.Event.MinutePassedRegisterd -= UpdateGameTime;
    }

    private void UpdateGameTime( )
    {
        // Update time
        Season gameSeason = TimeManager.Instance.GameSeason;
        int gameYear = TimeManager.Instance.GameYear;
        string gameDayOfWeek = TimeManager.Instance.GameDayOfWeek;
        int gameDay = TimeManager.Instance.GameDay;
        int gameHour = TimeManager.Instance.GameHour;
        int gameMinute = TimeManager.Instance.GameMinute;
        int gameSecond = TimeManager.Instance.GameSecond;

        gameMinute = gameMinute - (gameMinute % 10);

        string ampm = "";
        string minute;

        if (gameHour >= 12)
        {
            ampm = " pm";
        }
        else
        {
            ampm = " am";
        }

        if (gameHour >= 13)
        {
            gameHour -= 12;
        }

        if (gameMinute < 10)
        {
            minute = "0" + gameMinute.ToString();
        }
        else
        {
            minute = gameMinute.ToString();
        }

        string time = gameHour.ToString() + " : " + minute + ampm;
       _timeText.text = time;
       _dateText.text = gameDayOfWeek + ". " + gameDay.ToString();
        GameSeason = gameSeason;


        UpdateArrow();
    }

    void UpdateArrow()
    {
        float currentTime = CalculateGameTime(TimeManager.Instance.GameHour, TimeManager.Instance.GameMinute);

        float currentAngle = Mathf.Lerp(_maxValue, _minValue, ((currentTime - 360) / _totlaTime));

        _dayHand.rectTransform.eulerAngles = new Vector3(0f, 0f, currentAngle);
    }
    private void UpdateSeasonSprite()
    {
        switch (GameSeason)
        {
            case Season.SPRING:
                {
                    _seasonImage.sprite = Managers.Data.SpriteDict["TimeUI_2"];
                }
                break;
              
            case Season.SUMMER:
                {
                    _seasonImage.sprite = Managers.Data.SpriteDict["TimeUI_5"];
                }
                break;
            case Season.AUTUMN:
                {
                    _seasonImage.sprite = Managers.Data.SpriteDict["TimeUI_6"];
                }
                break;
            case Season.WINTER:
                {
                    _seasonImage.sprite = Managers.Data.SpriteDict["TimeUI_12"];
                }
                break;
        }

    }
    float CalculateGameTime(int hour, int minute)
    {
        return hour * 60f + minute;
    }
}
