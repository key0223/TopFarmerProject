using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerBar : MonoBehaviour
{
    Transform _hungerBar = null;
    void Awake()
    {
        _hungerBar = transform.Find("HungerBar").GetComponent<Transform>();
    }

    public void SetHungerBar(float ratio)
    {
        ratio = Mathf.Clamp(ratio, 0, 1);
        _hungerBar.localScale = new Vector3(ratio, 1.0f, 1.0f);
    }
}
