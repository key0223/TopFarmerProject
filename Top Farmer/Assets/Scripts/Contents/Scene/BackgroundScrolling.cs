using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundScrolling : MonoBehaviour
{
    public float _speed;
    public Transform[] _backgrounds;

    float _leftPosX = 0f;
    float _rightPosX = 0f;
    float _xScreenHalfSize;
    float _yScreenHalfSize;

    void Start()
    {
        _yScreenHalfSize = Camera.main.orthographicSize;
        _xScreenHalfSize = _yScreenHalfSize * Camera.main.aspect;

        // 화면의 왼쪽 끝 및 오른쪽 끝
        _leftPosX = -(_xScreenHalfSize) * 2;
        _rightPosX = _xScreenHalfSize * 4;
    }
    void Update()
    {
        for (int i = 0; i < _backgrounds.Length; i++)
        {
            _backgrounds[i].position += new Vector3(-_speed, 0, 0) * Time.deltaTime;

            if (_backgrounds[i].position.x < _leftPosX)
            {
                Vector3 nextPos = _backgrounds[i].position;
                nextPos = new Vector3(nextPos.x + _rightPosX, nextPos.y, nextPos.z);
                _backgrounds[i].position = nextPos;
            }
        }
    }
}

