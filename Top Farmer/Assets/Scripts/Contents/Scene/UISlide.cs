using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISlide : MonoBehaviour
{
    [SerializeField] GameObject _mainMenu;

    public RectTransform _toMoveUI;
    public Vector2 _hiddenPos;
    public Vector2 _visiblePos;
    public float _duration = 0.5f;

    float _sizeX;
    float _leftX;
    public float _rightX;

    private void Start()
    {
        _toMoveUI.anchoredPosition = _hiddenPos;
    }

    public void ShowUI()
    {
        StartCoroutine(CoSlideIn());
    }
    public void HideUI()
    {
        StartCoroutine(CoSlideOut());
    }
    IEnumerator CoSlideIn()
    {
        float elapsedTime = 0f;

        while(elapsedTime <_duration)
        {
            _toMoveUI.anchoredPosition = Vector2.Lerp(_hiddenPos, _visiblePos, elapsedTime / _duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _toMoveUI.anchoredPosition = _visiblePos;
    }
    IEnumerator CoSlideOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            _toMoveUI.anchoredPosition = Vector2.Lerp(_visiblePos, _hiddenPos,elapsedTime / _duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _toMoveUI.anchoredPosition = _hiddenPos;
        _mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
