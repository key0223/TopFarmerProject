using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;


// �� ���� �����Ǵ� ��� ������Ʈ�� �����մϴ�.
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
