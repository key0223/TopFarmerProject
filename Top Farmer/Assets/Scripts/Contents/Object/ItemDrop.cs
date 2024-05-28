using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemDrop : MonoBehaviour
{
    Coroutine _coDrop = null;
    Coroutine _coSearch = null;
    Coroutine _coLoot = null;
    

    public Vector3Int CellPos { get; private set; }

    #region Drop
    private float _gravity = -9.8f; // �߷� ���ӵ�
    private float _bounceDamping = 0.6f; // �ٿ ������
    private float _initialForceX = 1f; // �ʱ� x�� ��
    private float _initialForceY = 2f; // �ʱ� y�� ��
    private float _groundLevel = 0f; // �ٴ��� y�� ��ġ

    private Vector2 _velocity; // �ӵ�
    #endregion

    #region Loot

    [SerializeField]
    private GameObject _target;
    private int _speed =5;
    private int _searchRange = 2;
    #endregion


    private void Update()
    {
        if (_coSearch == null)
        {
            _coSearch = StartCoroutine(CoSearch());
        }
        else if (_target != null && _coLoot == null)
        {
            _coLoot = StartCoroutine(CoStartLoot());
        }
    }

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
                    _coDrop = null;
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
    IEnumerator CoSearch()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (_target != null)
                continue;

            _target = Managers.Object.FindCreature((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null) return false;

                Vector3Int dir = (pc.CellPos - CellPos);
                if (dir.magnitude > _searchRange) return false;

                if(_coDrop !=null)
                {
                    StopCoroutine(_coDrop);
                    _coDrop = null;
                }
                StartCoroutine("CoStartLoot");
                return true;
            });
        }
    }
    IEnumerator CoStartLoot()
    {
        while (_target != null)
        {
            ObjectController oc = _target.GetComponent<ObjectController>();

            Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(oc.CellPos) + new Vector3(0.5f, 0.5f);
            Vector3 dir = destPos - transform.position;

            float dist = dir.magnitude;
            if(dist<_speed*Time.deltaTime)
            {
                transform.position = destPos;
                
                // TODO : ������ �κ��丮 ���� �� ItemDrop GameObject ����
                StopAllCoroutines();
                Destroy(gameObject);
                yield break;
            }
            else
            {
                transform.position += dir.normalized * _speed * Time.deltaTime;
            }

            yield return null;
        }
    }
}