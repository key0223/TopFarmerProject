using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    Transform _hpBar = null;

    private void Awake()
    {
        _hpBar = transform.Find("HpBar").GetComponent<Transform>();

    }
    public void SetHpBar(float ratio)
    {
        ratio = Mathf.Clamp(ratio, 0, 1);
        _hpBar.localScale = new Vector3(ratio, 1.0f, 1.0f);
    }
}
