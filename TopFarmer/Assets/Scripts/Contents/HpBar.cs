using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image _hpBarImage;

    private void Start()
    {
        UpdateHpbar();
    }
    private void OnEnable()
    {
        Managers.Event.UpdateHpBarEvent -= UpdateHpbar;
        Managers.Event.UpdateHpBarEvent += UpdateHpbar;
    }
    private void OnDisable()
    {
        Managers.Event.UpdateHpBarEvent -= UpdateHpbar;
    }
    void UpdateHpbar()
    {
        float hpRatio = PlayerController.Instance.CurrentHp / PlayerController.Instance.MaxHp;
        _hpBarImage.fillAmount = hpRatio;

        // 체력 비율에 따라 색상 변화
        _hpBarImage.color = GetHpColor(hpRatio);
    }
    Color GetHpColor(float ratio)
    {
        if (ratio > 0.5f)
        {
            return Color.Lerp(Color.yellow, Color.green, (ratio - 0.5f) * 2);
        }
        else
        {
            return Color.Lerp(Color.red, Color.yellow, ratio * 2);
        }
    }
}
