using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MapChanger : MonoBehaviour
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Managers.Map.LoadMap("FarmerHouse");
        }
    }

}
