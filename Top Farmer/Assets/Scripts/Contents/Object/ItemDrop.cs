using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemDrop : MonoBehaviour
{
    Coroutine _coDrop = null;

    public Vector3Int CellPos { get; private set; }
    public float _gravity = -9.8f; // �߷� ���ӵ�
    public float _bounceDamping = 0.6f; // �ٿ ������
    public float _initialForceX = 2f; // �ʱ� x�� ��
    public float _initialForceY = 5f; // �ʱ� y�� ��
    public float _groundLevel = 0f; // �ٴ��� y�� ��ġ


    private Vector2 _velocity; // �ӵ�

    //private void Start()
    //{
    //    _velocity = new Vector2(Random.Range(-_initialForceX, _initialForceX), Random.Range(_initialForceY / 2, _initialForceY));
    //    _coDrop = StartCoroutine("CoStartItemDrop");
    //}

    public void OnDropItem(Vector3Int cellPos)
    {
        transform.position = Managers.Map.CurrentGrid.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0.0f);

        int randLevel = Random.Range(-1, 0);
        _groundLevel = Managers.Map.CurrentGrid.CellToWorld(cellPos).y+randLevel;
        _velocity = new Vector2(Random.Range(-_initialForceX, _initialForceX), Random.Range(_initialForceY / 2, _initialForceY));

        _coDrop = StartCoroutine("CoStartItemDrop");
    }

    IEnumerator CoStartItemDrop()
    {
        while (true)
        {
            // �ӵ��� �߷� ����
            _velocity.y += _gravity * Time.deltaTime;

            // ��ġ ������Ʈ
            Vector3 position = transform.position;
            position += (Vector3)_velocity * Time.deltaTime;

            // �ٴڿ� ������ �ٿ
            if (position.y <= _groundLevel)
            {
                position.y = _groundLevel;
                _velocity.y = -_velocity.y * _bounceDamping; // y�� �ӵ��� �����ϰ� ����

                // �ٿ�� ����� �۾����� ����
                if (Mathf.Abs(_velocity.y) < 0.1f)
                {
                    _velocity.y = 0;
                    break;
                }
            }

            transform.position = position;
            
            yield return null; // ���� �����ӱ��� ���
        }

        // ��ġ�� �ٽ� ����
        CellPos = Managers.Map.CurrentGrid.WorldToCell(transform.position);
        //transform.position = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
    }

}