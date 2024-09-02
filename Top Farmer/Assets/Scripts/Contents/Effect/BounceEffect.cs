using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect : MonoBehaviour
{
    float _time = 0;
    public float _size = 1;
    public float _sizeUpTime = 0.2f;

    private void Update()
    {
        if(_time<= _sizeUpTime)
        {
            transform.localScale = Vector3.one*(1 + _size * _time);
        }
        else if( _time<= _sizeUpTime*2)
        {
            transform.localScale = Vector3.one * (2 * _size * _sizeUpTime + 1 - _time * _size);
        }
        else
        {
            transform.localScale = Vector3.one;
        }

        _time += Time.deltaTime;
    }
}
