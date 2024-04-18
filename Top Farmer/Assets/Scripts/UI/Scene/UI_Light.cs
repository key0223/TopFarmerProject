using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Light : UI_Base
{
    enum Images
    {
        UI_Light,
    }
    enum Texts
    {
        MessageText,
    }

    Coroutine _coUpdateAlpha = null;

    float _dawnAlpha  = 0.3f;
    float _dayAlpha  = 0f;
    float _noonAlpha  = 0.3f;
    float _nightAlpha = 1f;
    //public float _nightAlpha = 0.85f;

    float transitionDuration = 1f;
    float _currentAlpha;
    float _targetAlpha;

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        GetText((int)Texts.MessageText).gameObject.SetActive(false);
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
                _currentAlpha = _nightAlpha;
                _targetAlpha = _dawnAlpha;

                if (_coUpdateAlpha != null)
                    return;
                GetText((int)Texts.MessageText).gameObject.SetActive(false);
                _coUpdateAlpha = StartCoroutine("CoUpdateAlpha");
                break;
            case DayState.Day:
                _currentAlpha = _dawnAlpha;
                _targetAlpha = _dayAlpha;
                if (_coUpdateAlpha != null)
                    return;
                _coUpdateAlpha = StartCoroutine("CoUpdateAlpha");
                break;
            case DayState.Noon:
                _currentAlpha = _dayAlpha;
                _targetAlpha = _noonAlpha;
                if (_coUpdateAlpha != null)
                    return;
                _coUpdateAlpha = StartCoroutine("CoUpdateAlpha");
                break;
            case DayState.Night:
                _currentAlpha = _noonAlpha;
                _targetAlpha = _nightAlpha;
                if (_coUpdateAlpha != null)
                    return;
                GetText((int)Texts.MessageText).gameObject.SetActive(true);
                _coUpdateAlpha = StartCoroutine("CoUpdateAlpha");
                break;
        }
    }

   IEnumerator CoUpdateAlpha()
    {
        Color color = GetImage((int)Images.UI_Light).color;

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            float alpha = Mathf.Lerp(_currentAlpha, _targetAlpha, t);
            
            color.a = alpha;
            GetImage((int)Images.UI_Light).color = color;

            yield return null;
        }
        _coUpdateAlpha = null;
    }
}
