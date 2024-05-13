using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Define;

public class CampfireController : ObjectController
{
    Light2D light;

    float _dayIntensity = 0.3f;
    float _dayRadiusIn = 0.2f;
    float _dayRadiusOut = 3f;
    float _dayStrength = 1;

    float _nightIntensity = 0.6f;
    float _nightRadiusIn = 0.2f;
    float _nightRadiusOut = 4f;
    float _nightStrength = 0.65f;

    float _transitionDuration = 2f;

    float _currentIntensity;
    float _currentRadiusIn;
    float _currentRadiusOut;
    float _currentStrength;

    float _targetIntensity;
    float _targetRadiusIn;
    float _targetRadiusOut;
    float _targetStrength;

    private DayState _state;
    public DayState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;
            _state = value;
            UpdateLight();

        }
    }
    protected override void Init()
    {
        base.Init();
        light = gameObject.GetComponentInChildren<Light2D>();
        Managers.Time.HourPassedRegistered -= UpdateState;
        Managers.Time.HourPassedRegistered += UpdateState;
        _state = Managers.Time.State;
    }

    void UpdateState()
    {
        State = Managers.Time.State;
    }
    void UpdateLight()
    {
        switch(State)
        {
            case DayState.Day:
                {
                    _currentIntensity = _nightIntensity;
                    _currentRadiusIn = _nightRadiusIn;
                    _currentRadiusOut = _nightRadiusOut;
                    _currentStrength = _nightStrength;

                    _targetIntensity = _dayIntensity;
                    _targetRadiusIn = _dayRadiusIn;
                    _targetRadiusOut = _dayRadiusOut;
                    _targetStrength = _dayStrength;
                    StartCoroutine("CoUpdateLight");
                }
                break;
            case DayState.Noon:
                {
                    _currentIntensity = _dayIntensity;
                    _currentRadiusIn = _dayRadiusIn;
                    _currentRadiusOut = _dayRadiusOut;
                    _currentStrength = _dayStrength;

                    _targetIntensity = _nightIntensity;
                    _targetRadiusIn = _nightRadiusIn;
                    _targetRadiusOut = _nightRadiusOut;
                    _targetStrength = _nightStrength;
                    StartCoroutine("CoUpdateLight");
                }
                break;
            case DayState.Night:
                {
                  
                }
                break;
        }
    }

    IEnumerator CoUpdateLight()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _transitionDuration;
            float intensity = Mathf.Lerp(_currentIntensity, _targetIntensity, t);
            float radiusIn = Mathf.Lerp(_currentRadiusIn, _targetRadiusIn, t);
            float radiusOut= Mathf.Lerp(_currentRadiusOut, _targetRadiusOut, t);
            float strength = Mathf.Lerp(_currentStrength, _targetStrength, t);

            light.intensity = intensity;
            light.pointLightInnerRadius= radiusIn;
            light.pointLightOuterRadius= radiusOut;
            light.falloffIntensity= strength;

            

            yield return null;
        }
    }
}
