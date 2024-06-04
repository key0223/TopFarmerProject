using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MapChanger : MonoBehaviour
{
    public int ObjectId { get; set; }
    public ObjectType ObjectType { get; set; }
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;

    private Vector3 _initPos;
    private string _connectedMap;
    private string _connectedPotal;
    public void InitPotal(PotalData data)
    {
        _connectedMap = data.connectedMap;
        _connectedPotal = data.connectedPotal;
        _initPos = new Vector3(data.potalPosX, data.potalPosY);
        CellPos = Managers.Map.CurrentGrid.WorldToCell(_initPos);
        Init();
    }

    protected virtual void Init()
    {
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Managers.Map.LoadMap($"{_connectedMap}");
            PotalData data = Managers.Data.PotalDict[$"{_connectedPotal}"];
            Vector3 pos = new Vector3(data.playerPosX, data.playerPosY);
            col.gameObject.transform.position = pos;

            Managers.Object.Player.CellPos = Managers.Map.CurrentGrid.WorldToCell(col.gameObject.transform.position);
        }
    }

}
