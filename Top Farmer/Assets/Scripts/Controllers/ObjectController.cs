using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;


// 맵 위에 생성되는 모든 오브젝트를 관리합니다.
public class ObjectController : MonoBehaviour
{
    public int ObjectId { get; set; }
    public SpriteRenderer _sprite;

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
}
