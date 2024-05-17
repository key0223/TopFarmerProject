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
    public ObjectType ObjectType { get; set; }
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    public SpriteRenderer _sprite;
   
    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
       
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }
}
