using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
    public void FadeOut()
    {
        StartCoroutine(CoFadeOut());
    }
    public void FadeIn()
    {
        StartCoroutine(CoFadeIn());
    }

    IEnumerator CoFadeOut()
    {
        float currentAlpha = _renderer.color.a;
        float distance = currentAlpha - Define.TargetAlpha;

        while (currentAlpha - Define.TargetAlpha > 0.01f)
        {
            currentAlpha = currentAlpha - distance / Define.FadeOutSecons * Time.deltaTime;
            _renderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }

        _renderer.color = new Color(1f, 1f, 1f, Define.TargetAlpha);
    }
    IEnumerator CoFadeIn()
    {
        float currentAlpha = _renderer.color.a;
        float distance = 1f - currentAlpha;

        while (1f - currentAlpha > 0.01f)
        {
            currentAlpha = currentAlpha + distance / Define.FadeInSeconds * Time.deltaTime;
            _renderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }

        _renderer.color = new Color(1f, 1f, 1f, 1f);
    }
}
