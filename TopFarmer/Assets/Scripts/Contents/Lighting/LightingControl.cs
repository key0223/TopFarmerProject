using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;
using static Define;

public class LightingControl : MonoBehaviour
{
    [SerializeField] LightingSchedule _lightingSchedule;
    [SerializeField] bool _isLightFlicker = false;
    [SerializeField][Range(0f, 1f)] float _lightFlickerIntensity;
    [SerializeField][Range(0f, 0.2f)] float _lightFlickerTimeMin;
    [SerializeField][Range(0f, 0.2f)] float _lightFlickerTimeMax;

    Light2D _light2D;
    Dictionary<string, float> _lightingBrightnessDictionary = new Dictionary<string, float>();
    float _currentLightIntensity;
    float _lightFlickerTimer = 0f;
    Coroutine _coFadeInLight;

    private void Awake()
    {
        _light2D = GetComponentInChildren<Light2D>();

        if (_light2D == null)
            enabled = false;

        foreach (LightingBrighteness lightingBrighteness in _lightingSchedule.lightingBrightenssArray)
        {
            string key = lightingBrighteness.season.ToString() + lightingBrighteness.hour.ToString();
            _lightingBrightnessDictionary.Add(key, lightingBrighteness.lightIntensity);
        }
    }

    private void OnEnable()
    {
        Managers.Event.HourPassedRegistered += HourPassedEvent;
        Managers.Event.AfterSceneLoadEvent += AfterSceneLoadEvent;
    }
    private void OnDisable()
    {
        Managers.Event.HourPassedRegistered -= HourPassedEvent;
        Managers.Event.AfterSceneLoadEvent -= AfterSceneLoadEvent;
    }

    private void Update()
    {
        if (_isLightFlicker)
            _lightFlickerTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (_lightFlickerTimer < 0f && _isLightFlicker)
        {
            LightFlicker();
        }
        else
        {
            _light2D.intensity = _currentLightIntensity;
        }
    }

    void HourPassedEvent()
    {
        Season season = TimeManager.Instance.GameSeason;
        int hour = TimeManager.Instance.GameHour;

        SetLightingIntensity(season, hour, true);
    }

    void AfterSceneLoadEvent()
    {
        SetLightingAfterSceneLoaded();
    }

    void SetLightingAfterSceneLoaded()
    {
        Season season = TimeManager.Instance.GameSeason;
        int hour = TimeManager.Instance.GameHour;

        SetLightingIntensity(season, hour, false);
    }

    void SetLightingIntensity(Season season, int hour, bool fadein)
    {
        int i = 0;

        while (i <= 23)
        {
            string key = season.ToString() + (hour).ToString();

            if (_lightingBrightnessDictionary.TryGetValue(key, out float targetLightingIntensity))
            {
                if (fadein)
                {
                    if (_coFadeInLight != null) StopCoroutine(_coFadeInLight);

                    _coFadeInLight = StartCoroutine(CoFadeIn(targetLightingIntensity));
                }
                else
                {
                    _currentLightIntensity = targetLightingIntensity;
                }

                break;
            }

            i++;
            hour--;

            if (hour < 0)
            {
                hour = 23;
            }
        }
    }

    IEnumerator CoFadeIn(float targetLightingIntensity)
    {
        float fadeDuration = 5f;

        float fadeSpeed = Mathf.Abs(_currentLightIntensity - targetLightingIntensity) / fadeDuration;
        while (!Mathf.Approximately(_currentLightIntensity, targetLightingIntensity))
        {
            _currentLightIntensity = Mathf.MoveTowards(_currentLightIntensity, targetLightingIntensity, fadeSpeed * Time.deltaTime);

            yield return null;
        }
        _currentLightIntensity = targetLightingIntensity;
    }

    void LightFlicker()
    {
        _light2D.intensity = Random.Range(_currentLightIntensity, _currentLightIntensity + (_currentLightIntensity * _lightFlickerIntensity));
        _lightFlickerTimer = Random.Range(_lightFlickerTimeMin, _lightFlickerTimeMax);
    }
}
