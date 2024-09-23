using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemNudge : MonoBehaviour
{
    WaitForSeconds _pause;
    bool _isAnimating = false;

    private void Awake()
    {
        _pause = new WaitForSeconds(0.04f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.position.x < collision.gameObject.transform.position.x)
        {
            StartCoroutine(CoRotateAntiClock());
        }
        else
        {
            StartCoroutine(CoRotateClock());
        }

        if(collision.gameObject.tag =="Player")
        {
            SoundManager.Instance.PlaySound(Define.Sound.SOUND_RUSTLE);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (transform.position.x > collision.gameObject.transform.position.x)
        {
            StartCoroutine(CoRotateAntiClock());
        }
        else
        {
            StartCoroutine(CoRotateClock());
        }

        if (collision.gameObject.tag == "Player")
        {
            SoundManager.Instance.PlaySound(Define.Sound.SOUND_RUSTLE);
        }

    }
    private IEnumerator CoRotateAntiClock()
    {
        _isAnimating = true;
        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            yield return _pause;
        }
        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return _pause;
        }
        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

        yield return _pause;
        _isAnimating = false;

    }
    private IEnumerator CoRotateClock()
    {
        _isAnimating = true;
        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return _pause;
        }
        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            yield return _pause;
        }
        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);

        yield return _pause;
        _isAnimating = false;
    }
}
