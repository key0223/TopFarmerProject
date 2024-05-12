using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static Define;

public class UI_Light : UI_Base
{
    enum Texts
    {
        MessageText,
    }
    enum GameObjects
    {
        GlobalLight
    }

    Light2D light;

    Coroutine _coUpdateAlpha = null;

    float _dawnIntensity = 5f;
    Color _dawnColor = new Color(0.1f, 0.08f, 0.15f);
    float _dayIntensity = 1.0f;
    Color _dayColor = Color.white;
    float _noonIntensity = 1.28f;
    Color _noonColor = new Color(0.83f, 0.57f, 0.31f);
    float _nightIntensity = 1.3f;
    Color _nightColor = new Color(0.05f, 0.057f, 0.11f);
   

    float transitionDuration = 3f;
    float _currentIntensity;
    float _targetIntensity;
    Color _currentColor;
    Color _targetColor;

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        GetText((int)Texts.MessageText).gameObject.SetActive(false);
        light =  GetObject((int)GameObjects.GlobalLight).GetComponent<Light2D>();
        
    }

    public void Start()
    {
        Managers.Time.Init();
        UpdateAlpha();
    }
    public void UpdateAlpha()
    {
        switch (Managers.Time.State)
        {
            case DayState.Dawn:
                _currentIntensity = _nightIntensity;
                _targetIntensity = _dawnIntensity;
                _currentColor = _nightColor;
                _targetColor = _dawnColor;

                if (_coUpdateAlpha != null)
                    return;
                GetText((int)Texts.MessageText).gameObject.SetActive(false);
                _coUpdateAlpha = StartCoroutine("CoUpdateAlpha");
                break;
            case DayState.Day:
                _currentIntensity = _dawnIntensity;
                _targetIntensity = _dayIntensity;
                _currentColor = _dawnColor;
                _targetColor = _dayColor;

                if (_coUpdateAlpha != null)
                    return;
                _coUpdateAlpha = StartCoroutine("CoUpdateAlpha");
                break;
            case DayState.Noon:
                _currentIntensity = _dayIntensity;
                _targetIntensity = _noonIntensity;
                _currentColor = _dayColor;
                _targetColor = _noonColor;

                if (_coUpdateAlpha != null)
                    return;
                _coUpdateAlpha = StartCoroutine("CoUpdateAlpha");
                break;
            case DayState.Night:

                _currentIntensity = _noonIntensity;
                _targetIntensity = _nightIntensity;
                _currentColor = _noonColor;
                _targetColor = _nightColor;
              
                if (_coUpdateAlpha != null)
                    return;
                GetText((int)Texts.MessageText).gameObject.SetActive(true);
                _coUpdateAlpha = StartCoroutine("CoUpdateAlpha");
                break;
        }
    }

   IEnumerator CoUpdateAlpha()
    {
        Color color = GetObject((int)GameObjects.GlobalLight).GetComponent<Light2D>().color;

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            float intensity = Mathf.Lerp(_currentIntensity, _targetIntensity, t);

            light.intensity = intensity;
            color = Color.Lerp(_currentColor, _targetColor, t);
            GetObject((int)GameObjects.GlobalLight).GetComponent<Light2D>().color = color;

            yield return null;
        }
        _coUpdateAlpha = null;
    }
}
